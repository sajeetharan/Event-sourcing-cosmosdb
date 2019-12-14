using System.Collections.Generic;
using AzureTableEventSourcingTest.Infrastructure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AzureTableEventSourcingTest.EventConsumers.AzureFunctions
{
    public static class HandleEvent
    {
        [FunctionName("HandleEvent")]
        public static void Run([CosmosDBTrigger(
            databaseName: "default",
            collectionName: "Flight",
            ConnectionStringSetting = "CosmosDb:ConnectionString",
			FeedPollDelay = 5000,
            LeaseCollectionName = "Leases",
			CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> input, ILogger logger)
        {
            if (input != null && input.Count > 0)
            {
				foreach (var document in input)
				{
                    var json = document.GetPropertyValue<JToken>("event");
					var @event = EventSerializer.FromJson(json);
					logger.LogInformation(@event.ToString());
				}
			}
        }
    }
}
