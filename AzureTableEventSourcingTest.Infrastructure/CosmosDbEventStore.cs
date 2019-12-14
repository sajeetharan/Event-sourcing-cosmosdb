using AzureTableEventSourcingTest.Domain;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Infrastructure
{
    public class CosmosDbEventStore<TId, TAggregateRoot> : IEventStore<TId, TAggregateRoot>, IInitializable
        where TAggregateRoot : IAggregateRoot<TId>
    {
        private static readonly TypeConverter aggregateRootIdTypeConverter = TypeDescriptor.GetConverter(typeof(TId));

        private readonly DocumentClient client;
        private readonly CosmosDbEventStoreSettings settings;

        public CosmosDbEventStore(DocumentClient client, CosmosDbEventStoreSettings settings)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        private string DatabaseId => settings.DatabaseName;
        private Uri DatabaseUri => UriFactory.CreateDatabaseUri(DatabaseId);
        private string CollectionId => typeof(TAggregateRoot).Name;
        private Uri CollectionUri => UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
        private string InsertEventsStoredProcedureId => "insertEvents";
        private Uri InsertEventsStoredProcedureUri => UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, InsertEventsStoredProcedureId);

        public async Task InitializeAsync()
        {
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId });
            await client.CreateDocumentCollectionIfNotExistsAsync(
                DatabaseUri, 
                new DocumentCollection
                {
                    Id = CollectionId,
                    PartitionKey = new PartitionKeyDefinition
                    {
                        Paths = { "/aggregateRootId" }
                    },
                    IndexingPolicy = new IndexingPolicy
                    {
                        IndexingMode = IndexingMode.Consistent,
                        Automatic = true,
                        IncludedPaths =
                        {
                            new IncludedPath
                            {
                                Path = "/versionNumber/?",
                                Indexes = { new RangeIndex(DataType.Number, precision: -1) }
                            }
                        },
                        ExcludedPaths =
                        {
                            new ExcludedPath { Path = "/" }
                        },
                    },
                },
                new RequestOptions
                {
                    OfferThroughput = 400,
                });

            await client.UpsertPartitionedStoredProcedure(
                CollectionUri,
                InsertEventsStoredProcedureUri,
                new StoredProcedure
                {
                    Id = InsertEventsStoredProcedureId,
                    Body = await GetType().Assembly.ReadEmbeddedResourceAsStringAsync("Resources.insertEvents.js"),
                });
        }

        public async Task CreateStreamAsync(TId id, IEnumerable<IEvent> events)
        {
            await AppendToStreamAsync(id, VersionNumber.None, events);
        }

        public async Task<(VersionNumber, IEnumerable<IEvent>)> ReadStreamAsync(TId aggregateRootId)
        {
            var aggregateRootIdAsString = aggregateRootIdTypeConverter.ConvertToInvariantString(aggregateRootId);
            var feedOptions = new FeedOptions
            {
                JsonSerializerSettings = EventSerializer.DefaultJsonSerializerSettings,
                PartitionKey = new PartitionKey(aggregateRootIdAsString),
            };

            var querySpec = new SqlQuerySpec($"SELECT * FROM {CollectionId} c ORDER BY c.versionNumber");
            var query = client.CreateDocumentQuery(CollectionUri, querySpec, feedOptions).AsDocumentQuery();
            
            var records = new List<EventRecord<TId>>();
            while (query.HasMoreResults)
            {
                var set = await query.ExecuteNextAsync<EventRecord<TId>>();
                records.AddRange(set);
            }

            if (records.Count == 0)
            {
                throw new StreamNotFoundException();
            }

            var versionNumber = records[records.Count - 1].VersionNumber;
            var events = records.Select(r => r.Event);
            return (new VersionNumber(versionNumber), events);
        }

        public async Task AppendToStreamAsync(TId aggregateRootId, VersionNumber versionNumber, IEnumerable<IEvent> events)
        {
            var aggregateRootIdAsString = aggregateRootIdTypeConverter.ConvertToInvariantString(aggregateRootId);
            var requestOptions = new RequestOptions
            {
                JsonSerializerSettings = EventSerializer.DefaultJsonSerializerSettings,
                PartitionKey = new PartitionKey(aggregateRootIdAsString),
            };
            try
            {
                await client.ExecuteStoredProcedureAsync<int>(
                    InsertEventsStoredProcedureUri,
                    requestOptions,
                    aggregateRootIdAsString,
                    versionNumber.Value,
                    events);
            }
            catch (DocumentClientException e) when (e.GetSubStatusCode() == 409 /* Conflict */)
            {
                throw new ConcurrencyException();
            }
        }
    }

    public class CosmosDbEventStoreSettings
    {
        public CosmosDbEventStoreSettings(string databaseName)
        {
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        }

        public string DatabaseName { get; }
    }
}
