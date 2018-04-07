using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace Functions
{
    public static class DeployReadyFunction
    {
        [FunctionName("DeployReady")]
        public static async Task<HttpResponseMessage> DeployReady(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestMessage req,
            [Table("OrchestratorStatus", Connection = "StagingStatusTableStorage")]CloudTable status,
            TraceWriter log)
        {
            log.Info("Start - DeployReadyFunction");
            var isTestValue = req.GetQueryNameValuePairs().Where(kv => kv.Key.ToLower() == "istest")
                .Select(kv => kv.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(isTestValue))
            {
                if (bool.TryParse(isTestValue, out var isTest))
                {
                    return req.CreateResponse(HttpStatusCode.OK, new DeployReadyResponse()
                    {
                        DeployStatus = isTest
                    });
                }
            }

            var query = new TableQuery<OrchestratorStatus>()
                .Take(1)
                ;

            TableContinuationToken token = null;
            var segment = await status.ExecuteQuerySegmentedAsync(query, token);
            var isRunning = false;
            foreach (var tableEntity in segment)
            {
                isRunning = true;
            }

            log.Info("Success - DeployReadyFunction");
            return req.CreateResponse(HttpStatusCode.OK,
                new DeployReadyResponse() { DeployStatus = !isRunning }
            );
        }
    }
}
