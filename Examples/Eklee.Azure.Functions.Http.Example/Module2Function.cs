using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http.Example
{
	public static class Module2Function
	{
		[ExecutionContextDependencyInjection(typeof(MyModule2))]
		[FunctionName("Module2Function1")]
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
