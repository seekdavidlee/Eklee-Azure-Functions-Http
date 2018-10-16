namespace Eklee.Azure.Functions.Http.Example
{
    public class SomeDomainB : ISomeDomainB
    {
        private readonly IMyRequestScope _requestScope;
        private readonly IMyHttpLogger _myHttpLogger;
        private readonly ISomeDomainA _someDomainA;

        public SomeDomainB(IMyRequestScope requestScope, IMyHttpLogger myHttpLogger, ISomeDomainA someDomainA)
        {
            _someDomainA = someDomainA;
            _requestScope = requestScope;
            _myHttpLogger = myHttpLogger;

            _myHttpLogger.LogMessage("Creating a new instance of SomeDomainB");
        }

        public void DoWork()
        {
            _someDomainA.DoWork();

            _myHttpLogger.LogMessage($"SomeDomainB work: {_requestScope.Id}");
        }
    }
}