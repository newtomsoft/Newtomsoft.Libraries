using System;

namespace Newtomsoft.EntityFramework.Exceptions
{
    public class RepositoryProviderException : Exception
    {
        public RepositoryProviderException(string message) : base(message)
        {
        }
    }
}
