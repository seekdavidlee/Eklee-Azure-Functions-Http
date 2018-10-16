using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http
{
    public interface IHttpRequestContext
    {
        HttpRequest Request { get; set; }
        ILogger Logger { get; set; }
    }
}
