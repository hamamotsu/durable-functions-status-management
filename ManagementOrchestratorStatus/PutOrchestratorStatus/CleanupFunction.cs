using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Functions
{
    public static class CleanupFunction
    {
        [FunctionName("Cleanup")]
        public static HttpResponseMessage Cleanup([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info($"Start - Cleanup");

            var isTestValue = req.GetQueryNameValuePairs().Where(kv => kv.Key.ToLower() == "istest")
                .Select(kv => kv.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(isTestValue))
            {
                if (bool.TryParse(isTestValue, out var isTest))
                {
                    return req.CreateResponse(HttpStatusCode.OK, new CleanupResponse()
                    {
                        Success = isTest
                    });
                }
            }

            log.Info($"Success - Cleanup");
            return req.CreateResponse(HttpStatusCode.OK, new CleanupResponse()
            {
                Success = true
            });
        }
    }
}
