using Basket.API.Services.Interfaces;
using Shared.Configurations;

namespace Basket.API.Services
{
    public class BasketEmailTemplateService : EmailTemplateService, IEmailTemplateService
    {
        private readonly BackgroundJobSettings backgroundJobSettings;
        public BasketEmailTemplateService(BackgroundJobSettings settings) : base(settings)
        {
            backgroundJobSettings = settings;
        }

        public string GenerateReminderCheckoutOrderEmail( string username)
        {
            var _checkoutUrl = $"{backgroundJobSettings.ApiGwUrl}/{backgroundJobSettings.BasketUrl}/{username}";
            var emailText = ReadEmailTemplateContent("reminder-checkout-order");
            var emailReplaceText = emailText.Replace("[username]",username).Replace("[checkoutUrl]", _checkoutUrl);
            return emailReplaceText;
        }
    }
}
