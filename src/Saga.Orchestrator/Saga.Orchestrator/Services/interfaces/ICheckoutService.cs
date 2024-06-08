using Shared.Dtos.Basket;

namespace Saga.Orchestrator.Services.interfaces
{
    public interface ICheckoutSagaService
    {
        Task<bool> CheckoutOrder(string username, BasketCheckoutDto basketCheckout);
    }
}
