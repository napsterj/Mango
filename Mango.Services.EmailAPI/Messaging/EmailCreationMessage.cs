using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Services;
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
            _serviceBusClient = new ServiceBusClient(_configuration.GetSection("ServiceBusSettings")
                                    .GetValue<string>("ConnectionString"));

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
