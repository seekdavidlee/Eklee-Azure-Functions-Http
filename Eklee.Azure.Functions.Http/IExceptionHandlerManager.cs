using System;
using Microsoft.AspNetCore.Mvc;

namespace Eklee.Azure.Functions.Http
{
    public interface IExceptionHandlerManager
    {
        IActionResult Handle(Exception ex);
    }
}
