using Autofac;

namespace Eklee.Azure.Functions.Http.Example
{
    public class MyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MyRequestScope>().As<IMyRequestScope>().InstancePerLifetimeScope();
            builder.RegisterType<SomeDomainA>().As<ISomeDomainA>();
            builder.RegisterType<SomeDomainB>().As<ISomeDomainB>();
            builder.RegisterType<SomeApiDomain>().As<ISomeApiDomain>();
            builder.RegisterType<MyHttpLogger>().As<IMyHttpLogger>().InstancePerLifetimeScope();
        }
    }
}
