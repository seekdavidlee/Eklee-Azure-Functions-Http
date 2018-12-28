using Autofac;

namespace Eklee.Azure.Functions.Http.Example
{
	public class MyModule4 : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SomeDomainAModule4>().As<ISomeDomainA>();
		}
	}
}