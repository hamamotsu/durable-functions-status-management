using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Functions
{
    public static class DeployToStagingFunction
    {
        [FunctionName("DeployToStaging")]
        public static async Task<HttpResponseMessage> DeployToStaging(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req,
            [Table("OrchestratorStatus", Connection = "DeployStatusTableStorage")]CloudTable deployStatus,
            TraceWriter log)
        {
            log.Info("Start - DeployToStaging");

            var body = await req.Content.ReadAsStringAsync();
            log.Info($"Body => {body}");

            var reqesutObject = JsonConvert.DeserializeObject<DeployToStagingRequest>(body);
            if (reqesutObject.IsTest.HasValue)
            {
                return req.CreateResponse(HttpStatusCode.OK, new DeployToStagingResponse()
                {
                    Success = reqesutObject.IsTest.Value
                });
            }

            TableOperation insertOperation = TableOperation.InsertOrReplace(new DeployStatusEntity()
            {
                PartitionKey = "Staging",
                RowKey = reqesutObject.DeployId
            });

            deployStatus.Execute(insertOperation);

            log.Info("Success - DeployToStaging");
            return req.CreateResponse(HttpStatusCode.OK, new DeployToStagingResponse()
            {
                Success = true
            });
        }
    }
}
