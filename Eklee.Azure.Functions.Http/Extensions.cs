using Microsoft.Azure.WebJobs;

namespace Eklee.Azure.Functions.Http
{
    public static class Extensions
    {
        public static Resolver GetResolver(this ExecutionContext executionContext)
        {
            return ResolverFactory.GetResolver(executionContext.InvocationId.ToString("N"));
        }

        public static T Resolve<T>(this ExecutionContext executionContext)
        {
            return executionContext.GetResolver().Get<T>();
        }
    }
}
