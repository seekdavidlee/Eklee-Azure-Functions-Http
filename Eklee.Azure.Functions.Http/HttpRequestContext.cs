using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Eklee.Azure.Functions.Http
{
    public class HttpRequestContext : IHttpRequestContext
    {
        public HttpRequest Request { get; set; }
        public ILogger Logger { get; set; }

        public T GetModelFromBody<T>()
        {
            if (Request != null)
            {
                using (var stream = new StreamReader(Request.Body))
                {
                    var body = stream.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(body);
                }
            }

            throw new ArgumentNullException();
        }
    }
}