# Introduction

The purpose of this library is to help developers with dependency injection per HTTP request context. This is currently not available via the default dependency injection infrastructure.

## DI Usage

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

## Dependency Resolution Usage:

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

## Azure Function Logger Usage:
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

## Exception Handling Usage:
To translate an exception-type to an HTTP response like Bad Request, you can implement the IExceptionHandler which will provide a way for the infrastructure code to recognize an exception type and return the correct HTTP response.

```
public class MyArgumentExceptionHandler : IExceptionHandler
{
    public ExceptionHandlerResult Handle(Exception ex)
    {
        if (ex.GetType() == typeof(ArgumentException))
        {
            return new ExceptionHandlerResult(true, new BadRequestObjectResult(new ErrorMessage { Message = ex.Message }));
        }

        return null;
    }
}
```

Remember to register the handler in your Module. You can register more than one.

```
builder.RegisterType<MyArgumentExceptionHandler>().As<IExceptionHandler>();
builder.RegisterType<MyCustomExceptionHandler>().As<IExceptionHandler>();
```

To leverage the handlers, you will need to use the extension method Run.

```
[ExecutionContextDependencyInjection(typeof(MyModule))]
[FunctionName("Function3")]
public static async Task<IActionResult> Run3(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
    HttpRequest req, ILogger log, ExecutionContext executionContext)
{
    // Example of how we can directly resolve a dependency.
    return await executionContext.Run<IDtoDomain, DtoResponse>(domain => Task.FromResult(domain.DoWork()));
}
```

## Azure AD integration

We can access user context from the Azure AD integration. Inject IHttpRequestContext into your domain and access the Security property. 

See the following link for more information on these special headers:
https://docs.microsoft.com/en-us/azure/app-service/app-service-authentication-how-to#access-user-claims


```
_httpRequestContext.Security.Principal.Name
...
_httpRequestContext.Security.Principal.Id
```

