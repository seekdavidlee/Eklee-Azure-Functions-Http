using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Eklee.Azure.Functions.Http
{
    public class ExceptionHandlerManager : IExceptionHandlerManager
    {
        private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;

        public ExceptionHandlerManager(IEnumerable<IExceptionHandler> exceptionHandlers)
        {
            _exceptionHandlers = exceptionHandlers;
        }

        public IActionResult Handle(Exception ex)
        {
            foreach (var exceptionHandler in _exceptionHandlers)
            {
                var result = exceptionHandler.Handle(ex);

                if (result != null && result.CanHandle)
                    return result.Result;
            }

            return null;
        }
    }
}