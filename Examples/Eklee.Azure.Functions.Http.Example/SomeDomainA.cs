namespace Eklee.Azure.Functions.Http.Example
{
    public class SomeDomainA : ISomeDomainA
    {
        private readonly IMyRequestScope _myRequestScope;
        private readonly IMyHttpLogger _myHttpLogger;

        public SomeDomainA(IMyRequestScope myRequestScope, IMyHttpLogger myHttpLogger)
        {
            _myRequestScope = myRequestScope;
            _myHttpLogger = myHttpLogger;
            _myHttpLogger.LogMessage("Creating a new instance of SomeDomainA");
        }

        public string DoWork()
        {
            _myHttpLogger.LogMessage($"SomeDomainA work: {_myRequestScope.Id}");
	        return "SomeDomainAModule1";
        }
    }
}
