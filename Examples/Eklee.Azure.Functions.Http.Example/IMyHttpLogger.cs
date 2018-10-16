using System.Collections.Generic;

namespace Eklee.Azure.Functions.Http.Example
{
    public interface IMyHttpLogger
    {
        void LogMessage(string message);
        IEnumerable<string> GetMessages();
    }
}
