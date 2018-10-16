using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http.Example
{
    public class MyHttpLogger : IMyHttpLogger
    {
        private readonly IHttpRequestContext _httpRequestContext;

        public MyHttpLogger(IHttpRequestContext httpRequestContext)
        {
            _httpRequestContext = httpRequestContext;
        }
        private readonly List<string> _messages = new List<string>();

        public void LogMessage(string message)
        {
            _messages.Add(message);
            _httpRequestContext.Logger.LogInformation(message);
        }

        public IEnumerable<string> GetMessages()
        {
            return _messages;
        }
    }
}