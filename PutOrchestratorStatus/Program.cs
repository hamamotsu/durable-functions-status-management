using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PutOrchestratorStatus
{
    public static class Program
    {
        [FunctionName("PutOrchestratorStatus")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get","post")]HttpRequestMessage req,
            [Table("OrchestratorStatus", Connection = "StatusTableStorage")]CloudTable status,
            TraceWriter log)
        {
            log.Info("C# HttpTrigger for EventGrid.");

            // parse query parameter
            var requestBody = await req.Content.ReadAsStringAsync();
            log.Info($"Body => {requestBody}");

            if (string.IsNullOrEmpty(requestBody))
            {
                return req.CreateResponse(HttpStatusCode.OK);
            }

            var messages = JsonConvert.DeserializeObject<JArray>(requestBody);

            //If the request is for subscription validation, send back the validation code.
            if (messages.Count > 0 && string.Equals((string)messages[0]["eventType"],
                    "Microsoft.EventGrid.SubscriptionValidationEvent",
                    System.StringComparison.OrdinalIgnoreCase))
            {
                log.Info("Validate request received");
                return req.CreateResponse(HttpStatusCode.OK,
                    new
                    {
                        validationResponse = messages[0]["data"]["validationCode"]
                    });
            }

            var batchOperation = new TableBatchOperation();

            foreach (JObject message in messages)
            {
                log.Info($"EventGridEvent: {message}");

                var data = message["Data"].ToObject<EventGridEventData>();

                switch (data.EventType)
                {
                    case (int)OrchestrationRuntimeStatus.Running:
                        batchOperation.InsertOrMerge(new OrchestratorStatus()
                        {
                            PartitionKey = data.InstanceId,
                            RowKey = Guid.NewGuid().ToString()
                        });
                        break;
                    case (int)OrchestrationRuntimeStatus.Completed:
                        var query = new TableQuery<OrchestratorStatus>()
                            .Where(TableQuery.GenerateFilterCondition(nameof(OrchestratorStatus.PartitionKey), QueryComparisons.Equal, data.InstanceId));

                        var segment = await status.ExecuteQuerySegmentedAsync(query, null);
                        foreach (var tableEntity in segment)
                        {
                            batchOperation.Delete(tableEntity);
                        }
                        break;
                    default:
                        break;
                }
            }

            await status.ExecuteBatchAsync(batchOperation, new TableRequestOptions
            {
                RetryPolicy = new LinearRetry(TimeSpan.FromMilliseconds(10), 5)
            }, null);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
