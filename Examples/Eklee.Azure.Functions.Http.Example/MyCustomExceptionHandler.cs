using System;
using Eklee.Azure.Functions.Http.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eklee.Azure.Functions.Http.Example
{
    public class MyCustomExceptionHandler : IExceptionHandler
    {
        public ExceptionHandlerResult Handle(Exception ex)
        {
            if (ex.GetType() == typeof(MyCustomException))
            {
                return new ExceptionHandlerResult(true, new BadRequestObjectResult(new ErrorMessage { Message = ex.Message }));
            }

            return null;
        }
    }
}