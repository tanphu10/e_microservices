using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;
namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;
        public BasketRepository(IDistributedCache redisCashedService, ISerializeService serializeSerive, ILogger logger)
        {
            _redisCacheService = redisCashedService;
            _serializeService = serializeSerive;
            _logger = logger;

        }
        public async Task<Cart?> GetBasketByUserName(string username)
        {
            _logger.Information($"BEGIN: GetBasketFromUserName {username}");
            var basket = await _redisCacheService.GetStringAsync(username);
            _logger.Information($"END: GetBasketFromUserName {username}");
            return string.IsNullOrEmpty(basket) ? null :
                _serializeService.Deserialize<Cart>(basket);
        }



        public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options)
        {
            _logger.Information($"BEGIN: UpdateBasket {cart.UserName}");

            if (options != null)
            {
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart), options);
            }
            else
            {
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
                _logger.Information($"END: UpdateBasket {cart.UserName}");

            }
            return await GetBasketByUserName(cart.UserName);
        }
        public async Task<bool> DeleteBasketFromUserName(string username)
        {
            try
            {
                _logger.Information($"BEGIN: DeleteBasketFromUserName {username}");
                await _redisCacheService.RemoveAsync(username);
                _logger.Information($"END DeleteBasketFromUserName {username}");

                return true;
            }
            catch (Exception e)
            {
                _logger.Error("DeleteBasketFromUserName" + e.Message);
                throw;
            }

        }

    }
}
