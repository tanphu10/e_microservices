using Contracts.Sagas.OrderManager;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.interfaces;
using Shared.Dtos.Basket;

namespace Saga.Orchestrator.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureService(this IServiceCollection services) =>
            services.AddTransient<ICheckoutSagaService, CheckoutSagaService>()
            .AddTransient<ISagaOrderManager<BasketCheckoutDto,OrderResponse>,SagaOrderManager>();
        public static IServiceCollection ConfigureHttpRepository(this IServiceCollection services) =>
            services.AddScoped<IOrderHttpRepository, OrderHttpRepository>()
            .AddScoped<IBasketHttpRepository, BasketHttpRepository>()
            .AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();

        public static void ConfigureHttpClients(this IServiceCollection services)
        {

            ConfigureOrderHttpClient(services);
            ConfigureBasketHttpClient(services);
            ConfigureInventoryHttpClient(services);
        }
        private static void ConfigureOrderHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrdersAPi", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5005/api/v1/");
            });
            services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("OrdersAPI"));
        }
        private static void ConfigureBasketHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketsAPi", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5004/api/");
            });
            services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("BasketsAPi"));
        }
        private static void ConfigureInventoryHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryAPi", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5006/api/");
            });
            services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("InventoryAPi"));
        }
    }
}
