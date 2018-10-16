using System;
using Autofac;
using Autofac.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http
{
    internal static class AutoFacScopes
    {
        internal static void Register(string instanceId, Type moduleType, HttpRequest httpRequest, ILogger logger)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule((IModule)Activator.CreateInstance(moduleType));
            builder.RegisterType<HttpRequestContext>().As<IHttpRequestContext>().InstancePerLifetimeScope();

            var container = builder.Build();
            var scope = container.BeginLifetimeScope();

            var httpRequestContext = scope.Resolve<IHttpRequestContext>();
            httpRequestContext.Logger = logger;
            httpRequestContext.Request = httpRequest;

            ResolverFactory.Add(instanceId, scope);
        }

        internal static void Unregister(string instanceId)
        {
            ResolverFactory.Remove(instanceId);
        }
    }
}
