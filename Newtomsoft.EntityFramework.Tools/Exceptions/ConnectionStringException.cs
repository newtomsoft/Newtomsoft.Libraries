using System;

namespace Newtomsoft.EntityFramework.Tools.Exceptions
{
    public class ConnectionStringException : Exception
    {
        public ConnectionStringException(string message) : base(message)
        {
        }
    }
}
