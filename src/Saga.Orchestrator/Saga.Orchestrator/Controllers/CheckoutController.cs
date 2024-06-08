using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.Services.interfaces;
using Shared.Dtos.Basket;
using System.ComponentModel.DataAnnotations;

namespace Saga.Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutSagaService _checkoutSagaService;
     
        public CheckoutController(ICheckoutSagaService checkoutSagaService)
        {
            _checkoutSagaService = checkoutSagaService;
            
        }
        [HttpPost]
        [Route("{username}")]
        public async Task<IActionResult> CheckoutOrder([Required] string username,[FromBody] BasketCheckoutDto model)
        {
            var result = await _checkoutSagaService.CheckoutOrder(username, model);
            return Ok(result);

        }
    }
}
