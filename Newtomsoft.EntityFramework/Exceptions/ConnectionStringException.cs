using System;

namespace Newtomsoft.EntityFramework.Exceptions
{
    public class ConnectionStringException : Exception
    {
        public ConnectionStringException(string message) : base(message)
        {
        }
    }
}
