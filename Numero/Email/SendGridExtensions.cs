using Microsoft.Extensions.DependencyInjection;
using Numero.Services;

namespace Numero.Email
{
    public static class SendGridExtensions
    {
       public static IServiceCollection  AddSendGridEmailSender( this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, SendGridEmailSender>();

            return services;
        }
    }
}
