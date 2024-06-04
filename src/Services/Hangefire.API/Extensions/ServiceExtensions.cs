using Contracts.ScheduledJobs;
using Infrastructure.ScheduledJobs;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configurations;
using System.Runtime.CompilerServices;

namespace Hangefire.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var hangfireSettings = configuration.GetSection(nameof(HangFireSettings)).Get<HangFireSettings>();
            services.AddSingleton(hangfireSettings);
            return services;
        }
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
            => services.AddTransient<IScheduledJobService, HangfireService>();

    }
}
