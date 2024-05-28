using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Interfaces;
using Infrastructure.Common;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;

namespace Basket.API.Extensitons
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// trong này chúng ta sẽ tạo một cái tổng quát hóa để có thể lấy data từ appsetting.json
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
                .Get<EventBusSettings>();
            services.AddSingleton(eventBusSettings);
            var cacheSettings = configuration.GetSection(nameof(CacheSettings))
              .Get<CacheSettings>();
            services.AddSingleton(cacheSettings);
            return services;
        }
        public static IServiceCollection ConfigureServices(this IServiceCollection services) => services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializeService, SerializeService>();
        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            //var redisConnectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;
            var settings = services.GetOptions<CacheSettings>("CacheSettings");
            if (string.IsNullOrEmpty(settings.ConnectionString))
                throw new ArgumentNullException("Redis connection string is not configured");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = settings.ConnectionString;
            });
        }
        public static void ConfigureMassTransit(this IServiceCollection services)
        {
            var settings = services.GetOptions<EventBusSettings>("EventBusSettings");
            if (settings == null || string.IsNullOrEmpty(settings.HostAddress)) throw new ArgumentNullException("EventBusSetting is not configured");
            var mqConnection = new Uri(settings.HostAddress);
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            // BasketCheckoutEvent=->>> basket-checkout-event
            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(mqConnection);
                });
                config.AddRequestClient<IBasketCheckoutEvent>();
            });

        }
    }
}
