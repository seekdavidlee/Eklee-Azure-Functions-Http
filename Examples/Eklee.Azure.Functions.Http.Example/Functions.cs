using System.Threading.Tasks;
using Eklee.Azure.Functions.Http.Example.Models;
using Eklee.Azure.Functions.Http.Models;
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

        [ExecutionContextDependencyInjection(typeof(MyModule))]
        [FunctionName("Function5")]
        public static async Task<IActionResult> Run5(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            // Example of how we can directly resolve a dependency.
            return await executionContext.Run<IMyLogDomain, string>(domain => Task.FromResult(domain.DoWork()));
        }

        [ExecutionContextDependencyInjection(typeof(MyModule))]
        [FunctionName("Function6")]
        public static async Task<IActionResult> Run6(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            // Example of how we can directly resolve a dependency.
            return await executionContext.Run<IDomainWithCache, CacheResult<KeyValueDto>>(async domain => await domain.GetAsync(req.Query["key"]));
        }

	    [ExecutionContextDependencyInjection(typeof(MyModule))]
	    [FunctionName("Module1Function1")]
	    public static IActionResult Run(
		    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
		    HttpRequest req, ILogger log, ExecutionContext executionContext)
	    {
		    // Example of how we can access the resolver to resolve a dependency.
		    var resolver = executionContext.GetResolver();
		    return new OkObjectResult(new { Message = resolver.Get<ISomeDomainA>().DoWork() });
	    }
	}
}
