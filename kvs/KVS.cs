using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System.Web;
using System.Text;

namespace AMAUpdateSample
{
    public class KVS
    {
        private static Dictionary<string, string> _inMemoryKVS = new Dictionary<string, string>();

        private readonly ILogger _logger;

        public KVS(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<KVS>();
        }

        [Function("set")]
        public async Task<HttpResponseData> Set([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("SET invoked");


            var querystring = HttpUtility.ParseQueryString(req.Url.Query);
            var key = querystring["key"];

            if (String.IsNullOrWhiteSpace(key)) {
                _logger.LogWarning("missing key in querystring");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var value = await req.ReadAsStringAsync();

            _inMemoryKVS[key] = value ?? string.Empty;

            return req.CreateResponse(HttpStatusCode.NoContent);
        }

        [Function("get")]
        public HttpResponseData Get([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("GET invoked");

            var querystring = HttpUtility.ParseQueryString(req.Url.Query);
            var key = querystring["key"];

            if (String.IsNullOrWhiteSpace(key)) {
                _logger.LogWarning("missing key in querystring");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var value = string.Empty;
            if (_inMemoryKVS.ContainsKey(key)) {
                value = _inMemoryKVS[key];
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(value));
                return response;
            } else {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    
        [Function("version")]
        public HttpResponseData Version([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("VERSION invoked");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Body = new MemoryStream(Encoding.UTF8.GetBytes("v2"));

            return response;
        }
    }
}
