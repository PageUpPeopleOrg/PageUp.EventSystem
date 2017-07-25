using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PageUp.Events;

namespace BusinessEvents.SubscriptionEngine.Core.Notifiers
{
    public class SlackNotifier : INotifier
    {
        private readonly ISubscriptionsManager subscriptionsManager;

        public SlackNotifier(ISubscriptionsManager subscriptionsManager)
        {
            this.subscriptionsManager = subscriptionsManager;
        }
        public async Task Notify(Subscription subscriber, Message message, Event @event)
        {
            var slackText = new
            {
                text = JsonConvert.SerializeObject(message)
            };

            var payloadJson = JsonConvert.SerializeObject(slackText);
           
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var response = await httpclient.PostAsync(subscriber.Endpoint, new StringContent(payloadJson, Encoding.UTF8, "application/json"));

                    if (!response.IsSuccessStatusCode)
                    {
                        subscriptionsManager.RecordErrorForSubscriber(subscriber, message, @event, response);
                    }
                }
                catch (Exception exception)
                {
                    subscriptionsManager.RecordErrorForSubscriber(subscriber, message, @event, exception);
                }
            }
        }
    }
}