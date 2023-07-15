using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlockTimer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlockTimer(this IServiceCollection services, Action<BlockTimerOptions>? configureOptions = default)
    {
        if (configureOptions is null)
        {
            services.Configure<BlockTimerOptions>(o =>
            {
                o.WarningThreshold = long.MaxValue;
            });
        }
        else
        {
            services.Configure(configureOptions);
        }

        services.AddSingleton<IBlockTimer>(sp =>
        {
            var logger = sp.GetService<ILogger<BlockTimer>>();
            logger ??= LoggerFactory.Create(b => b.AddConsole()).CreateLogger<BlockTimer>();

            var options = sp.GetRequiredService<IOptions<BlockTimerOptions>>();

            var blockTimer = new BlockTimer(logger, options);

            return blockTimer;
        });

        return services;
    }
}
