﻿using Common.Logging;
using Hangfire;
using Serilog;
using Shared.Configurations;

namespace Hangefire.API.Extensions
{
    public static class HostExtensions
    {
        public static void AddAppConfigurations(this ConfigureHostBuilder host)
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            }).UseSerilog(Serilogger.Configure);
        }
        internal static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration)
        {
            var configDashboard = configuration.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>();
            var hangfireSettings = configuration.GetSection("HangfireSettings").Get<HangFireSettings>();
            var hangfireRoute = hangfireSettings.Route;
            app.UseHangfireDashboard(hangfireRoute, new DashboardOptions
            {
                //Authorization = new[] {}
                DashboardTitle = configDashboard.DashboardTitle,
                StatsPollingInterval = configDashboard.StatsPollingInterval,
                AppPath = configDashboard.AppPath,
                IgnoreAntiforgeryToken = true
            });
            return app;
        }
    }
}