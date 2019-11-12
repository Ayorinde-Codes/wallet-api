using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using Numero.Email;
using Numero.Helpers;

namespace Numero.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly AppSettings _appsettings;

        public SendGridEmailSender(IOptions<AppSettings> appSettings)
        {
            _appsettings = appSettings.Value;
        }

        public async Task<SendEmailResponse> SendEmailAsync(string userEmail, string emailSubject, string message)
        {
            var apiKey = _appsettings.SendGridKey;
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("noreply@email.com", "TransRoute"));
            msg.AddTo(new EmailAddress(userEmail, "New User"));
            msg.SetTemplateId(_appsettings.TemplateId);
            
            var dynamicTemplateData = new ExampleTemplateData
            {
                FirstName = emailSubject,
                Otp = message
            };

            msg.SetTemplateData(dynamicTemplateData);
            var response = await client.SendEmailAsync(msg);

            return new SendEmailResponse();
        }

        private class ExampleTemplateData
        {
            [JsonProperty("firstName")]
            public string FirstName { get; set; }

            [JsonProperty("otp")]
            public string Otp { get; set; }

        }
    }
}

