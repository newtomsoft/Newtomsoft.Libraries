using System;

namespace Newtomsoft.EntityFramework.Tests;

public class FixtureEnvironment : IDisposable
{
    private const string Environment = "NewtomsoftEntityFrameworkTestEnvironment";
    private readonly string _environmentSavedValue;
    public FixtureEnvironment()
    {
        _environmentSavedValue = System.Environment.GetEnvironmentVariable(Environment, EnvironmentVariableTarget.User);
    }

    public void Dispose()
    {
        System.Environment.SetEnvironmentVariable(Environment, _environmentSavedValue, EnvironmentVariableTarget.User);
    }
}