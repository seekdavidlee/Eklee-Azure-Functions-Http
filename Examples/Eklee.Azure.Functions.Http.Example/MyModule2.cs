using Autofac;

namespace Eklee.Azure.Functions.Http.Example
{
	public class MyModule2 : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SomeDomainAModule2>().As<ISomeDomainA>();
		}
	}
}