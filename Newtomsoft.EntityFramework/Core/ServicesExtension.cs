namespace Newtomsoft.EntityFramework.Core;

public static class ServicesExtension
{
    public static void AddDbContext<T>(this IServiceCollection services, IConfiguration configuration) where T : DbContext
        => EntityFrameworkTools<T>.AddDbContext(services, configuration);
}