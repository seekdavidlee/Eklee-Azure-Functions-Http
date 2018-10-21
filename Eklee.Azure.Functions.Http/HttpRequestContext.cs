using System;
using System.IO;
using Eklee.Azure.Functions.Http.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Eklee.Azure.Functions.Http
{
    public class HttpRequestContext : IHttpRequestContext
    {
        // ReSharper disable once InconsistentNaming
        public const string X_MS_CLIENT_PRINCIPAL_NAME = "X-MS-CLIENT-PRINCIPAL-NAME";
        // ReSharper disable once InconsistentNaming
        public const string X_MS_CLIENT_PRINCIPAL_ID = "X-MS-CLIENT-PRINCIPAL-ID";

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

        public Security Security
        {
            get
            {
                var security = new Security { Principal = new Principal() };

                if (Request.Headers.ContainsKey(X_MS_CLIENT_PRINCIPAL_NAME))
                {
                    security.Principal.Name = Request.Headers[X_MS_CLIENT_PRINCIPAL_NAME];
                }

                if (Request.Headers.ContainsKey(X_MS_CLIENT_PRINCIPAL_ID))
                {
                    security.Principal.Id = Request.Headers[X_MS_CLIENT_PRINCIPAL_ID];
                }

                return security;
            }
        }
    }
}