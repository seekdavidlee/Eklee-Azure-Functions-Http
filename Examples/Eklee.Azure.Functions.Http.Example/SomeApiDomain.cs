using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Eklee.Azure.Functions.Http.Example
{
    public class SomeApiDomain : ISomeApiDomain
    {
        private readonly ISomeDomainA _someDomainA;
        private readonly ISomeDomainB _someDomainB;
        private readonly IMyHttpLogger _myHttpLogger;

        public SomeApiDomain(
            ISomeDomainA someDomainA,
            ISomeDomainB someDomainB, IMyHttpLogger myHttpLogger)
        {
            _someDomainA = someDomainA;
            _someDomainB = someDomainB;
            _myHttpLogger = myHttpLogger;
        }

        public Task<IActionResult> DoWork()
        {
            _someDomainA.DoWork();
            _someDomainB.DoWork();

            return Task.FromResult<IActionResult>(new OkObjectResult(_myHttpLogger.GetMessages()));
        }
    }
}