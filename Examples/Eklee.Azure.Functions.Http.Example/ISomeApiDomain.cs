using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Eklee.Azure.Functions.Http.Example
{
    public interface ISomeApiDomain
    {
        Task<IActionResult> DoWork();
    }
}
