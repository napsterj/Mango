using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string serviceBusConnectionString = "Endpoint=sb://mangowebapplication.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Zy7Jgin6/hCJ35+XM+46kkv0q0BYDUhpX+ASbP+AIaA=";

        public MessageBus() { }
        public async Task PostMessageToBus(object message, string queueName)
        {
            try
            {
                await using var client = new ServiceBusClient(serviceBusConnectionString);

                ServiceBusSender sender = client.CreateSender(queueName);

                var finalMessage = JsonConvert.SerializeObject(message);

                ServiceBusMessage sbmessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(finalMessage));

                sbmessage.CorrelationId = Guid.NewGuid().ToString();

                await sender.SendMessageAsync(sbmessage);
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);  
            }
        }
    }
}
