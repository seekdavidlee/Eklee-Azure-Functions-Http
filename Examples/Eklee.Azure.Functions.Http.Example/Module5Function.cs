using System.Threading.Tasks;
using Eklee.Azure.Functions.Http.Example.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http.Example
{
	public static class Module5Function
	{
		[ExecutionContextDependencyInjection(typeof(MyModule5))]
		[FunctionName("Module5Function1")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
			HttpRequest req, ILogger log, ExecutionContext executionContext)
		{
			return await executionContext.Run<IDtoDomain, DtoResponse>(domain => Task.FromResult(domain.DoWork()));
		}
	}
}
