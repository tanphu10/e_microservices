﻿using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
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
        public BasketsController(IBasketRepository repository)
        {
            _repository = repository;
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
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

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
    }
}