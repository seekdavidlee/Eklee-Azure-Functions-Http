namespace Eklee.Azure.Functions.Http.Example
{
    public class MyRequestScope : IMyRequestScope
    {
        public MyRequestScope(IMyHttpLogger httpLogger, IHttpRequestContext httpRequestContext)
        {
            httpLogger.LogMessage("Creating a new instance of MyRequestScope");
            Id = httpRequestContext.Request.Headers["X-SomeKey"];
            httpLogger.LogMessage($"Setting: {Id}");
        }

        public string Id { get; }
    }
}