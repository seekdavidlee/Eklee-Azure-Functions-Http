using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http
{
#pragma warning disable 618
    public class ExecutionContextDependencyInjectionAttribute : Attribute, IFunctionInvocationFilter
#pragma warning restore 618
    {
        private readonly Type _moduleType;
        private const string ScopeName = "afscope";

        public ExecutionContextDependencyInjectionAttribute(Type moduleType)
        {
            _moduleType = moduleType;
        }

#pragma warning disable 618
        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
#pragma warning restore 618
        {
            var instanceId = (string)executedContext.Properties[ScopeName];
            AutoFacScopes.Unregister(instanceId);
            return Task.CompletedTask;
        }

#pragma warning disable 618
        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
#pragma warning restore 618
        {
            ILogger logger = null;
            HttpRequest httpRequest = null;
            Microsoft.Azure.WebJobs.ExecutionContext executionContext = null;

            executingContext.Arguments.Keys.ToList().ForEach(key =>
            {
                var o = executingContext.Arguments[key];

                if (o.GetType().GetInterfaces().Any(x => x == typeof(ILogger)))
                {
                    logger = (ILogger)o;
                }
                else if (o.GetType().BaseType == typeof(HttpRequest))
                {
                    httpRequest = (HttpRequest)o;
                }
                else if (o is Microsoft.Azure.WebJobs.ExecutionContext context)
                {
                    executionContext = context;
                }
            });

            var instanceId = executingContext.FunctionInstanceId.ToString("N");
            AutoFacScopes.Register(instanceId, _moduleType, httpRequest, logger, executionContext);
            executingContext.Properties.Add(ScopeName, instanceId);
            return Task.CompletedTask;
        }
    }
}
