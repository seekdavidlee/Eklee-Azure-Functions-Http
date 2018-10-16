using System.Collections.Generic;
using Autofac;

namespace Eklee.Azure.Functions.Http
{
    internal static class ResolverFactory
    {
        private static readonly Dictionary<string, ILifetimeScope> Scopes = new Dictionary<string, ILifetimeScope>();

        internal static Resolver GetResolver(string instanceId)
        {
            return new Resolver(Scopes[instanceId]);
        }

        internal static void Add(string instanceId, ILifetimeScope scope)
        {
            Scopes.Add(instanceId, scope);
        }

        internal static void Remove(string instanceId)
        {
            Scopes[instanceId].Dispose();
            Scopes.Remove(instanceId);
        }
    }
}