using Autofac;

namespace Eklee.Azure.Functions.Http.Example
{
	public class MyModule3 : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SomeDomainAModule3>().As<ISomeDomainA>();
		}
	}
}