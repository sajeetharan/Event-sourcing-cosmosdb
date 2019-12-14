using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Infrastructure
{
    public static class DocumentClientExtensions
    {
        public static int? GetSubStatusCode(this DocumentClientException exception)
        {
            const string subStatusHeaderName = "x-ms-substatus";

            var valueSubStatus = exception.ResponseHeaders.Get(subStatusHeaderName);
            if (string.IsNullOrEmpty(valueSubStatus))
            {
                return null; 
            }

            return int.TryParse(valueSubStatus, NumberStyles.Integer, CultureInfo.InvariantCulture, out var subStatusCode)
                ? subStatusCode
                : (int?)null;
        }



        public static async Task UpsertPartitionedStoredProcedure(this DocumentClient documentClient, Uri collectionUri, Uri storedProcedureUri, StoredProcedure storedProcedure)
        {
            try
            {
                await documentClient.CreateStoredProcedureAsync(
                    collectionUri,
                    storedProcedure);
            }
            catch (DocumentClientException dex) when (dex.StatusCode == HttpStatusCode.Conflict)
            {
                await documentClient.ReplaceStoredProcedureAsync(
                    storedProcedureUri,
                    storedProcedure);
            }
        }
    }
}
