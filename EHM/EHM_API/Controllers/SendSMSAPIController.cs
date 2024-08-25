using EHM_API.DTOs;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendSMSAPIController : ControllerBase
    {

        [HttpPost]
        public IActionResult SendSms()
        {
            var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber("+84965865133"));
            messageOptions.From = new PhoneNumber("+16194302266");
            messageOptions.Body = "123456";

            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);


            return Ok(message);
        }
    }
}