using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IEmailTemplateService _emailTemlateService;
        public BasketsController(IBasketRepository repository, IMapper mapper, IPublishEndpoint publishEndpoint, StockItemGrpcService stockItemGrpcServices,IEmailTemplateService emailTemplateService)
        {
            _repository = repository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _stockItemGrpcServices = stockItemGrpcServices;
            _emailTemlateService = emailTemplateService;
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
        public async Task<ActionResult<Cart>> UpdateBasket([FromBody] Cart cart)
        {
            //Communicate with inventory.Grpc  and check quantity available of products
            foreach (var item in cart.Items)
            {
                var stock = await _stockItemGrpcServices.GetStock(item.ItemNo);
                item.SetAvailableQuantity(stock.Quantity);

            }
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(10));
            //.SetSlidingExpiration(TimeSpan.FromMinutes(10));

            var result = await _repository.UpdateBasket(cart, options);
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
        [HttpPost("[action]", Name = "SendEmailReminder")]
        public ContentResult SendEmailReminder()
        {
            var emailTemplate = _emailTemlateService.GenerateReminderCheckoutOrderEmail("phantanphu10@gmail.com", username: "test");
            var result = new ContentResult
            {
                Content = emailTemplate,
                ContentType = "text/html"
            };
            return result;

        }


    }
}
