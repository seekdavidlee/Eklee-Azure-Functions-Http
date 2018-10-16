using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http
{
    public class HttpRequestContext : IHttpRequestContext
    {
        public HttpRequest Request { get; set; }
        public ILogger Logger { get; set; }
    }
}