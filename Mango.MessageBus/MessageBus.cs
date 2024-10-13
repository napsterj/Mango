using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
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
        private string keyVaultUrl = "https://key-vault-mango-dev.vault.azure.net/";
                
        //It is created in Azure App Registrations service..
        private string DirectoryId = "206ffd2d-6232-4fe6-be4c-ff02d7f18867";
        private string ClientId = "cf2926c6-a613-4efc-b353-503f2eccc79c";
        private string ClientSecret = ".MR8Q~tKKxaHd4sDNeRb-I3HXeNKNYYJFNYm-cLR";

        public MessageBus() { }
        public async Task PostMessageToBus(object message, string queueName)
        {
            try
            {
                
                var clientCreds = new ClientSecretCredential(DirectoryId, ClientId, ClientSecret);
                var clientSecret = new SecretClient(new Uri(keyVaultUrl), clientCreds);
                var kv = await clientSecret.GetSecretAsync("kv-dev-servicebus-conn-string");
                
                await using var client = new ServiceBusClient(kv.Value.ToString());

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
