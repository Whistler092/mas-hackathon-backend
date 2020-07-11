using FirebaseAdmin.Messaging;
using MAS.Hackathon.BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Hackathon.BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushNotificationController : ControllerBase
    {
        private readonly IOptions<PushNotificationConfig> options;
        private readonly FirebaseMessaging messaging;

        public PushNotificationController(IOptions<PushNotificationConfig> options, FirebaseMessaging messaging)
        {
            this.options = options;
            this.messaging = messaging;
        }

        [HttpPost]
        [Route("Subscribe")]
        public async Task<IActionResult> Subscribe([FromBody]IDictionary<string, string> payload)
        {
            try
            {
                await messaging.SubscribeToTopicAsync(payload.Values.ToList(), options.Value.Topic);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> Send()
        {
            var img = "https://static01.nyt.com/newsgraphics/2020/03/10/coronavirus-pathogen/0ee8f1a44b1932072653a0a75ff96486feb6f36d/covid-virus.png";

            try
            {
                var message = new Message()
                {
                    Topic = options.Value.Topic,
                    Webpush = new WebpushConfig
                    {
                        FcmOptions = new WebpushFcmOptions
                        {
                            Link = "https://hackathon-gotzero.web.app/"
                        },
                        Notification = new WebpushNotification
                        {
                            Data = new Dictionary<string, string>()
                            {
                                ["AnyKey"] = "AnyValue"
                            },
                            Title = "Message Title",
                            Body = "Message Body",
                            Image = img,
                            Icon = img
                        }
                    }
                };
                await messaging.SendAsync(message);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}