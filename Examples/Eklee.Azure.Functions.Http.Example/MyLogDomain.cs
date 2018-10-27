using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http.Example
{
    public class MyLogDomain : IMyLogDomain
    {
        private readonly ILogger _logger;

        public MyLogDomain(ILogger logger)
        {
            _logger = logger;
        }

        public string DoWork()
        {
            _logger.LogInformation("MyLogDomain DoWork invoked.");

            return "MyLogDomain DoWork invoked.";
        }
    }
}