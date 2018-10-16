# Introduction

The purpose of this library is to help developers with dependency injection per HTTP request context. This is currently not available via the default dependency injection infrastructure.

## Usage

In order to leverage this library, there are 3 steps. You would want to setup your DI, apply the ExecutionContextDependencyInjection attribute, and inject the ExecutionContext as a parameter in your function.

### Step 1: Setup DI

The first step is to setup your DI via the Autofac Module. 

```
using Autofac;

namespace FunctionApp1
{
    public class MyModuleConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MyDomain>().As<IMyDomain>();
        }
    }
}
```

### Step 2/3: Setup ExecutionContextDependencyInjection attribute on said function and inject ExecutionContext.

The second step is to apply the ExecutionContextDependencyInjection on your function and tell it which Module type you would like. Next, you can inject the ExecutionContext which internally carries the function instance Id.

```
public static class Function1
{
    [ExecutionContextDependencyInjection(typeof(MyModuleConfig))]
    [FunctionName("Function1")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log,
        ExecutionContext executionContext)
    {
```

## Usage:

Now, there are 2 extension methods you can use against the ExecutionContext. You can choose the Resolve method to resolve your dependency.

```
var myDomain = executionContext.Resolve<IMyDomain>();
myDomain.DoWork();
```

Or you can get the Resolver.

```
var resolver = executionContext.GetResolver();
var myDomain = resolver.Get<IMyDomain>();
myDomain.DoWork();
```

## Domain usage:
Note that we have already captured ILogger as well as the actual HttpRequest. So, if you can inject IHttpRequestContext to access the logger or Http request which is hanging off IHttpRequestContext.

```
public class MyDomain : IMyDomain
{
    private readonly IHttpRequestContext _requestContext;

    public MyDomain(IHttpRequestContext requestContext)
    {
        _requestContext = requestContext;
    }

    public void DoWork()
    {
        _requestContext.Logger.LogInformation("FOOBAR");
    }
}
```