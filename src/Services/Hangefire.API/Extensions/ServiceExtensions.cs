using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangefire.API.Services;
using Hangefire.API.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.ScheduledJobs;
using Infrastructure.Services;
using Shared.Configurations;

namespace Hangefire.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var hangfireSettings = configuration.GetSection(nameof(HangFireSettings)).Get<HangFireSettings>();
            services.AddSingleton(hangfireSettings);
            var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting)).Get<SMTPEmailSetting>();
            services.AddSingleton(emailSettings);
            return services;
        }
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
            => services.AddTransient<IScheduledJobService, HangfireService>().AddTransient<ISmtpEmailService,SmtpEmailService>().AddTransient<IBackgroundJobService,BackgroundJobService>();

    }
}
