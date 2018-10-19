using System;
using Eklee.Azure.Functions.Http.Models;

namespace Eklee.Azure.Functions.Http
{
    public interface IExceptionHandler
    {
        ExceptionHandlerResult Handle(Exception ex);
    }
}
