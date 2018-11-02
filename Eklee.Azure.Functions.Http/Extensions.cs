using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Autofac;
using Eklee.Azure.Functions.Http.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Eklee.Azure.Functions.Http
{
    public static class Extensions
    {
        public static Resolver GetResolver(this ExecutionContext executionContext)
        {
            return ResolverFactory.GetResolver(executionContext.InvocationId.ToString("N"));
        }

        public static T Resolve<T>(this ExecutionContext executionContext)
        {
            return executionContext.GetResolver().Get<T>();
        }

        public static async Task<IActionResult> Run<TDomain, TOutput>(this ExecutionContext executionContext, Func<TDomain, Task<TOutput>> action)
        {
            var domain = executionContext.GetResolver().Get<TDomain>();

            try
            {
                return new OkObjectResult(await action(domain));
            }
            catch (ValidationException validationException)
            {
                return new BadRequestObjectResult(new ErrorMessage { Message = validationException.Message });
            }
            catch (Exception e)
            {
                var errorHandlerManager = executionContext.Resolve<IExceptionHandlerManager>();

                var result = errorHandlerManager.Handle(e);

                if (result != null) return result;

                // Unhandled exception, throw.
                throw;
            }
        }

        public static ContainerBuilder UseDistributedCache<TDistributedCache>(this ContainerBuilder containerBuilder) where TDistributedCache : IDistributedCache
        {
            containerBuilder.RegisterType<CacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<TDistributedCache>().As<IDistributedCache>().SingleInstance();

            // AddOptions()

            containerBuilder.RegisterGeneric(typeof(OptionsManager<>)).As(typeof(IOptions<>)).SingleInstance();
            containerBuilder.RegisterGeneric(typeof(OptionsManager<>)).As(typeof(IOptionsSnapshot<>)).InstancePerLifetimeScope();
            containerBuilder.RegisterGeneric(typeof(OptionsMonitor<>)).As(typeof(IOptionsMonitor<>)).SingleInstance();
            containerBuilder.RegisterGeneric(typeof(OptionsFactory<>)).As(typeof(IOptionsFactory<>));
            containerBuilder.RegisterGeneric(typeof(OptionsCache<>)).As(typeof(IOptionsMonitorCache<>)).SingleInstance();
            return containerBuilder;
        }
    }
}
