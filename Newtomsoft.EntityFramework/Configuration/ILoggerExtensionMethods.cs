using Microsoft.Extensions.Logging;

namespace Newtomsoft.EntityFramework.Configuration
{
    public static class ILoggerExtensionMethods
    {
        public static ILoggingBuilder AddCustom(this ILoggingBuilder builder, string value)
        {
            return value switch
            {
                NewtomsoftConfiguration.CONSOLE_OUTPUT => builder.AddConsole(),
                NewtomsoftConfiguration.DEBUG_OUTPUT => builder.AddDebug(),
                NewtomsoftConfiguration.JSONCONSOLE_OUTPUT => builder.AddJsonConsole(),
                _ => builder,
            };
        }
    }

}
