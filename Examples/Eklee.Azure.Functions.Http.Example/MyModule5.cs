using System.Linq;
using Autofac;
using Eklee.Azure.Functions.Http.Example.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace Eklee.Azure.Functions.Http.Example
{
	public class ConfigParameter : IJwtTokenValidatorParameters
	{
		public ConfigParameter(IConfiguration configuration)
		{
			Audience = configuration.GetValue<string>("Audience");
			Issuers = configuration.GetValue<string>("Issuers").Split(' ');
		}
		public string Audience { get; }
		public string[] Issuers { get; }
	}

	public class Module5DtoDomain : IDtoDomain
	{
		private readonly IHttpRequestContext _requestContext;

		public Module5DtoDomain(IHttpRequestContext requestContext)
		{
			_requestContext = requestContext;
		}

		public DtoResponse DoWork()
		{
			var usernameClaim = _requestContext.Security.ClaimsPrincipal.Claims.Single(x =>
				x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn");

			return new DtoResponse { Key1Result = $"User is: {usernameClaim.Value}" };
		}
	}

	public class MyModule5 : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<Module5DtoDomain>().As<IDtoDomain>().InstancePerLifetimeScope();
			builder.UseJwtAuthorization<ConfigParameter>();
			builder.UseDistributedCache<MemoryDistributedCache>();
		}
	}
}
