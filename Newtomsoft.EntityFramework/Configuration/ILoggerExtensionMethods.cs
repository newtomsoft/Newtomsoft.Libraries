using Microsoft.Extensions.Logging;

namespace Newtomsoft.EntityFramework.Configuration;

public static class LoggerExtensionMethods
{
    public static ILoggingBuilder AddCustom(this ILoggingBuilder builder, string value)
    {
        return value switch
        {
            NewtomsoftConfiguration.ConsoleOutput => builder.AddConsole(),
            NewtomsoftConfiguration.DebugOutput => builder.AddDebug(),
            NewtomsoftConfiguration.JsonconsoleOutput => builder.AddJsonConsole(),
            _ => builder,
        };
    }
}