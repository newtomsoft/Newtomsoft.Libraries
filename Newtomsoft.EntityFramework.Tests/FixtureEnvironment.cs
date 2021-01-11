using Newtomsoft.EntityFramework.Constants;
using System;

namespace Newtomsoft.EntityFramework.Tests
{
    public class FixtureEnvironment : IDisposable
    {
        public readonly string SaveEnvironment;
        public FixtureEnvironment()
        {
            SaveEnvironment = Environment.GetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, EnvironmentVariableTarget.User);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, SaveEnvironment, EnvironmentVariableTarget.User);
        }
    }
}
