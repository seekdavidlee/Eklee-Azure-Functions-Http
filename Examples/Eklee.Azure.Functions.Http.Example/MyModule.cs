using Autofac;
using Microsoft.Extensions.Caching.Distributed;

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
            builder.RegisterType<DtoDomain>().As<IDtoDomain>();
            builder.RegisterType<ConfigDomain>().As<IConfigDomain>();
            builder.RegisterType<MyLogDomain>().As<IMyLogDomain>();

            builder.RegisterType<MyArgumentExceptionHandler>().As<IExceptionHandler>();
            builder.RegisterType<MyCustomExceptionHandler>().As<IExceptionHandler>();

            builder.RegisterType<DomainWithCache>().As<IDomainWithCache>();
            builder.UseDistributedCache<MemoryDistributedCache>();
        }
    }
}
