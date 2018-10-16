using Autofac;

namespace Eklee.Azure.Functions.Http
{
    public class Resolver
    {
        private readonly ILifetimeScope _lifetimeScope;

        public Resolver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public T Get<T>()
        {
            return _lifetimeScope.Resolve<T>();
        }
    }
}