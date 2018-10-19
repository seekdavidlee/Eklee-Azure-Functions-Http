using System;
using Microsoft.AspNetCore.Mvc;

namespace Eklee.Azure.Functions.Http.Models
{
    public class ExceptionHandlerResult
    {
        public ExceptionHandlerResult(bool canHandle, IActionResult result = null)
        {
            CanHandle = canHandle;

            if (canHandle && result == null) throw new ArgumentNullException();

            Result = result;
        }

        public bool CanHandle { get; }

        public IActionResult Result { get; }
    }
}
