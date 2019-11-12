using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Numero.Helpers;
using Numero.Model;

namespace Numero.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BankListController:Controller
    {
         
        private readonly PaystackAppSettings _payStackAppSettings;

        public BankListController(IOptions<PaystackAppSettings> payStackAppSettings)
        {
            _payStackAppSettings = payStackAppSettings.Value;
        }

        [HttpGet]
        [Route("banks")]
        public async Task<IActionResult> City()
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync("https://api.paystack.co/bank");
            return Ok(response);
        }
        

        [HttpGet]
        [Route("verify")]
        public async Task <IActionResult> Verifyaccount(BankVerifyModel model)
        {
            var accountNumber = model.AccountNumber;
            var bankCode = model.BankCode;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _payStackAppSettings.TestSecret);
            var response = await client.GetStringAsync($"https://api.paystack.co/bank/resolve?account_number={accountNumber}&bank_code={bankCode}");
            return Ok(response);
        }
    
    }

}

