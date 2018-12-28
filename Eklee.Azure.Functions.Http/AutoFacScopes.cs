using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Eklee.Azure.Functions.Http
{
	internal static class AutoFacScopes
	{
		private static readonly Dictionary<string, IContainer> Containers =
			new Dictionary<string, IContainer>();

		internal static void Register(string instanceId, Type moduleType,
			HttpRequest httpRequest, ILogger logger, ExecutionContext executionContext)
		{
			IContainer container;

			string key = moduleType.FullName;

			// ReSharper disable once AssignNullToNotNullAttribute
			if (!Containers.ContainsKey(key))
			{
				var builder = new ContainerBuilder();
				builder.RegisterModule((IModule)Activator.CreateInstance(moduleType));
				builder.RegisterType<HttpRequestContext>().As<IHttpRequestContext>().InstancePerLifetimeScope();
				builder.RegisterType<ExceptionHandlerManager>().As<IExceptionHandlerManager>();

				builder.Register(c => new ConfigurationBuilder()
					.SetBasePath(executionContext.FunctionAppDirectory)
					.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
					.AddEnvironmentVariables()
					.Build()).As<IConfiguration>();

				builder.Register(c => logger).As<ILogger>().InstancePerLifetimeScope();

				container = builder.Build();
				Containers.Add(key, container);
			}
			else
			{
				container = Containers[key];

			}

			var scope = container.BeginLifetimeScope();

			var httpRequestContext = scope.Resolve<IHttpRequestContext>();
			httpRequestContext.Logger = logger;
			httpRequestContext.Request = httpRequest;

			ResolverFactory.Add(instanceId, scope);
		}

		// ReSharper disable once IdentifierTypo
		internal static void Unregister(string instanceId)
		{
			ResolverFactory.Remove(instanceId);
		}
	}
}
