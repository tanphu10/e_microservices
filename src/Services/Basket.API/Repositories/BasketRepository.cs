using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services.Interfaces;
using Basket.API.Services;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;
using Shared.Dtos.ScheduledJob;
using Infrastructure.Extensions;
namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;
        private readonly BackgroundJobHttpService _backgroundJobHttp;
        private readonly IEmailTemplateService _emailTemplateService;
        public BasketRepository(IDistributedCache redisCashedService, ISerializeService serializeSerive, ILogger logger, BackgroundJobHttpService backgroundJobHttp, IEmailTemplateService emailTemplateService)
        {
            _redisCacheService = redisCashedService;
            _serializeService = serializeSerive;
            _logger = logger;
            _backgroundJobHttp = backgroundJobHttp;
            _emailTemplateService = emailTemplateService;

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
            DeleteReminderCheckoutOrder(cart.UserName);

            _logger.Information($"BEGIN: UpdateBasket {cart.UserName}");

            if (options != null)
            {
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart), options);
            }
            else
            {
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));

            }
            _logger.Information($"END: UpdateBasket {cart.UserName}");
            try
            {
                await TriggerSendEmailReminderCheckout(cart);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);

            }
            return await GetBasketByUserName(cart.UserName);
        }


        private async Task TriggerSendEmailReminderCheckout(Cart cart)
        {
            var emailTemplate = _emailTemplateService.GenerateReminderCheckoutOrderEmail(cart.UserName);
            var model = new ReminderCheckoutOrderDto(cart.EmailAddress, "reminder checkout", emailTemplate, DateTimeOffset.UtcNow.AddMinutes(2));
            var uri = $"{_backgroundJobHttp.ScheduledJobUrl}/send-email-reminder-checkout-order";
            var response = await _backgroundJobHttp.Client.PostAsJson(uri, model);
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                var jobId = await response.ReadContentAs<string>();
                if (!string.IsNullOrEmpty(jobId))
                {
                    cart.JobId = jobId;
                    await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));

                }
            }
        }
        private async Task DeleteReminderCheckoutOrder(string username)
        {
            var cart = await GetBasketByUserName(username);
            if (cart == null || string.IsNullOrEmpty(cart.JobId)) return;
            var jobId = cart.JobId;
            var uri = $"{_backgroundJobHttp.ScheduledJobUrl}/delete/jobId/{jobId}";
            _backgroundJobHttp.Client.DeleteAsync(uri);
            _logger.Information($"DeleteReminderCheckoutOrder:Deleted JobId :{jobId}");
        }
        public async Task<bool> DeleteBasketFromUserName(string username)
        {
            DeleteReminderCheckoutOrder(username);
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
