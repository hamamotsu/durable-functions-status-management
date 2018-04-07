using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Functions
{
    public static class SwapFunction
    {
        [FunctionName("Swap")]
        public static HttpResponseMessage Swap(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("Start - SwapFunction");
            var isTestValue = req.GetQueryNameValuePairs().Where(kv => kv.Key.ToLower() == "istest")
                .Select(kv => kv.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(isTestValue))
            {
                if (bool.TryParse(isTestValue, out var isTest))
                {
                    return req.CreateResponse(HttpStatusCode.OK, new SwapResponse()
                    {
                        Success = isTest,
                        ProductionDeployId = "TestProductionDeployId",
                        StagingDeployId = "TestStagingDeployId"
                    });
                }
            }

            var res = new SwapResponse()
            {
                Success = true,
                ProductionDeployId = "SampleProductionId",
                StagingDeployId = "SampleStagingProductionId"
            };
            log.Info("Success - SwapFunction");

            return req.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
