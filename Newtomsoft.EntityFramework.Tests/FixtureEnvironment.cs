namespace Newtomsoft.EntityFramework.Tests;

public class FixtureEnvironment : IDisposable
{
    private const string ENVIRONMENT = "NewtomsoftEntityFrameworkTestEnvironment";
    public readonly string EnvironmentSavedValue;
    public FixtureEnvironment()
    {
        EnvironmentSavedValue = Environment.GetEnvironmentVariable(ENVIRONMENT, EnvironmentVariableTarget.User);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(ENVIRONMENT, EnvironmentSavedValue, EnvironmentVariableTarget.User);
    }
}