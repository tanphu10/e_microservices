using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Dtos.Basket;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;
        private readonly StockItemGrpcService _stockItemGrpcServices;
      

        public BasketsController(IBasketRepository repository, IMapper mapper, IPublishEndpoint publishEndpoint, StockItemGrpcService stockItemGrpcServices)
        {
            _repository = repository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _stockItemGrpcServices = stockItemGrpcServices;
           
        }


        [HttpGet("{username}", Name = "GetBasket")]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Cart>> GetBasket([Required] string username)
        {
            var result = await _repository.GetBasketByUserName(username);

            return Ok(result ?? new Cart(username));
        }



        [HttpPost(Name = "UpdateBasket")]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CartDto>> UpdateBasket([FromBody] CartDto model)
        {
            //Communicate with inventory.Grpc  and check quantity available of products
            foreach (var item in model.Items)
            {
                var stock = await _stockItemGrpcServices.GetStock(item.ItemNo);
                item.SetAvailableQuantity(stock.Quantity);

            }
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(10));
            //.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            var updateCart = _mapper.Map<Cart>(model);

            var cart = await _repository.UpdateBasket(updateCart, options);
            var result = _mapper.Map<CartDto>(cart);
            return Ok(result);
        }



        [HttpDelete("{username}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteBasket([Required] string username)
        {
            var result = await _repository.DeleteBasketFromUserName(username);
            return Ok(result);
        }


        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var basket = await _repository.GetBasketByUserName(basketCheckout.UserName);
            if (basket == null) return NotFound();

            //publish checkout event to Event Message
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;

            _publishEndpoint.Publish(eventMessage);
            //remove basket
            await _repository.DeleteBasketFromUserName(basketCheckout.UserName);
            return Accepted();
        }
      


    }
}
