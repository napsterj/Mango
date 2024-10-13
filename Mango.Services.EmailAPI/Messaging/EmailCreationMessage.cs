using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using Mango.Services.EmailAPI.Services;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class EmailCreationMessage : IEmailCreationMessage
    {
        private ServiceBusClient _serviceBusClient;
        private ServiceBusProcessor _serviceBusProcessor;
        private IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public EmailCreationMessage(IConfiguration configuration, IEmailService emailService) 
        { 
            _configuration = configuration;
            _emailService = emailService;
            
            var tenantId = _configuration.GetSection("KeyVaultSettings").GetValue<string>("DirectoryId");
            var clientId = _configuration.GetSection("KeyVaultSettings").GetValue<string>("ClientId");
            var clientSecret = _configuration.GetSection("KeyVaultSettings").GetValue<string>("ClientSecret");
            var kvUrl = _configuration.GetSection("KeyVaultSettings").GetValue<string>("KVUrl");
            var servieBusSecret = _configuration.GetSection("KeyVaultSettings").GetValue<string>("ServiceBusSecret");
            
            var kvClientSecret = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var kv = new SecretClient(new Uri(kvUrl), kvClientSecret);

            var connstring = kv.GetSecret(servieBusSecret).Value;
            
            _serviceBusClient = new ServiceBusClient(connstring.Value);


            _serviceBusProcessor = _serviceBusClient.CreateProcessor(_configuration.GetSection("ServiceBusSettings")
                                                    .GetValue<string>("NewEmailRegistered"));
        }
        public async Task Start()
        {
            try
            {
                _serviceBusProcessor.ProcessMessageAsync += OnMessageSendAsync;
                _serviceBusProcessor.ProcessErrorAsync += OnErrorMessageAsync;
                await _serviceBusProcessor.StartProcessingAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task OnErrorMessageAsync(ProcessErrorEventArgs args)
        {
            var message = args.Exception.ToString();
            await Task.CompletedTask;
        }

        private async Task OnMessageSendAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var serviceBusReceivedMessage = args.Message;
                if (serviceBusReceivedMessage != null) 
                {
                   var newRegisteredEmail = Encoding.ASCII.GetString(serviceBusReceivedMessage.Body);
                   await _emailService.LogNewCreatedUserEmail(newRegisteredEmail);
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task Stop()
        {
           await _serviceBusProcessor.StopProcessingAsync();
           await _serviceBusProcessor.DisposeAsync();
        }
    }
}
