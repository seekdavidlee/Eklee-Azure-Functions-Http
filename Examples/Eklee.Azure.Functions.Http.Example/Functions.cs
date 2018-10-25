using System.Threading.Tasks;
using Eklee.Azure.Functions.Http.Example.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http.Example
{
    public static class Functions
    {
        [ExecutionContextDependencyInjection(typeof(MyModule))]
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run1(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            // Example of how we can access the resolver to resolve a dependency.
            var resolver = executionContext.GetResolver();
            return await resolver.Get<ISomeApiDomain>().DoWork();
        }

        [ExecutionContextDependencyInjection(typeof(MyModule))]
        [FunctionName("Function2")]
        public static async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            // Example of how we can directly resolve a dependency.
            return await executionContext.Resolve<ISomeApiDomain>().DoWork();
        }

        [ExecutionContextDependencyInjection(typeof(MyModule))]
        [FunctionName("Function3")]
        public static async Task<IActionResult> Run3(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            // Example of how we can directly resolve a dependency.
            return await executionContext.Run<IDtoDomain, DtoResponse>(domain => Task.FromResult(domain.DoWork()));
        }

        [ExecutionContextDependencyInjection(typeof(MyModule))]
        [FunctionName("Function4")]
        public static async Task<IActionResult> Run4(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            // Example of how we can directly resolve a dependency.
            return await executionContext.Run<IConfigDomain, bool>(domain => Task.FromResult(domain.IsLocalEnvironment()));
        }
    }
}
