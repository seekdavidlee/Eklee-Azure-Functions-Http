using System;

namespace Eklee.Azure.Functions.Http.Example
{
    public class MyCustomException : Exception
    {
        public MyCustomException(string message) : base(message)
        {

        }
    }
}
