using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Text;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MpesaController : ControllerBase
    {
        [HttpPost]
        [Route("MakePayment")]
        public IActionResult MakePayment()
        {
            var client = new RestClient("https://sandbox.safaricom.co.ke/");

            var consumerKey = "hLXChgniGzGEEACtzm2aCAvh90QpKQJz"; // Your consumer key
            var consumerSecret = "fQwzQ5bdzGhl0dsJ"; // Your consumer secret
            var authString = $"{consumerKey}:{consumerSecret}";
            var base64AuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));

            client.AddDefaultHeader("Authorization", $"Basic {base64AuthString}");

            var request = new RestRequest("mpesa/stkpush/v1/processrequest", Method.Post);

            var BusinessShortCode = ""; // Your Business Shortcode
            var Passkey = ""; // Your Passkey
            var PartyA = "254796868506"; // Your phone number
            var AccountReference = ""; // Your Account Reference
            var TransactionDesc = "Test Payment";
            var Amount = ""; // Amount from your form input
            var Timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(BusinessShortCode + Passkey + Timestamp));

            request.AddJsonBody(new
            {
                BusinessShortCode = BusinessShortCode,
                Password = Password,
                Timestamp = Timestamp,
                TransactionType = "CustomerPayBillOnline",
                Amount = Amount,
                PartyA = PartyA,
                PartyB = BusinessShortCode,
                passkey = Passkey,
                PhoneNumber = PartyA,
                CallBackURL = "https://mydomain.com/path", // Your callback URL
                AccountReference = AccountReference,
                TransactionDesc = TransactionDesc
            });

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject(response.Content);
                // Handle the response result as needed
            }
            else
            {
                // Handle other status codes or errors
            }

            return Ok();
        }
    }
}
