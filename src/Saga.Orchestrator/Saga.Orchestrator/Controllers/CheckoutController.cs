using Contracts.Sagas.OrderManager;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.OrderManager;
using Shared.Dtos.Basket;
using Shared.Dtos.Inventory;
using System.ComponentModel.DataAnnotations;

namespace Saga.Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        //private readonly ICheckoutSagaService _checkoutSagaService;

        private readonly ISagaOrderManager<BasketCheckoutDto, OrderResponse> _sagaOrderManager;

        public CheckoutController(ISagaOrderManager<BasketCheckoutDto, OrderResponse> sagaOrderManager)
        {
            _sagaOrderManager = sagaOrderManager;

        }
        [HttpPost]
        [Route("{username}")]
        public OrderResponse CheckoutOrder([Required] string username, [FromBody] BasketCheckoutDto model)
        {
            //var result = await _sagaOrderManager.CheckoutOrder(username, model);
            model.UserName = username;
            var result = _sagaOrderManager.CreateOrder(model);
            return result ;

        }
    }
}
