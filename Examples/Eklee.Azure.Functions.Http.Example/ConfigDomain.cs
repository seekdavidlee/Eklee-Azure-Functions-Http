using Microsoft.Extensions.Configuration;

namespace Eklee.Azure.Functions.Http.Example
{
    public class ConfigDomain : IConfigDomain
    {
        private readonly IConfiguration _configuration;

        public ConfigDomain(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsLocalEnvironment()
        {
            var value = _configuration.GetValue<string>("AzureWebJobsStorage");
            return value == "UseDevelopmentStorage=true";
        }
    }
}
