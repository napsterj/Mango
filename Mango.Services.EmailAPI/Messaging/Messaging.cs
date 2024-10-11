
using Azure.Messaging.ServiceBus;
using Mango.Services.Email.API.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class Messaging : IMessaging
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly IOptions<ServiceBusSettings> _serviceBusSetting;
        private readonly IEmailService _emailService;

        public Messaging(IOptions<ServiceBusSettings> serviceBusSetting, IEmailService emailService) 
        { 
           _serviceBusSetting = serviceBusSetting;
            _emailService = emailService;
           // _client = new ServiceBusClient(_serviceBusSetting.Value.ConnectionString);
           //_processor = _client.CreateProcessor(_serviceBusSetting.Value.MessageQueue);
        }
        
        private Task OnErrorMessageAsyc(ProcessErrorEventArgs args)
        {
            var errorMessage = args.Exception.ToString();
            return Task.CompletedTask;
        }

        private async Task OnProcessMessageAsyc(ProcessMessageEventArgs args)
        {
            try
            {
                var message = args.Message;
                var body = Encoding.UTF8.GetString(message.Body);
                var cartDto = JsonConvert.DeserializeObject<CartDto>(body);

                //send email
                await _emailService.EmailCart(cartDto);
                await args.CompleteMessageAsync(message);
            }
            catch (Exception) 
            {
                throw;
            }
        }

        public async Task Start()
        {
            _processor.ProcessMessageAsync += OnProcessMessageAsyc;
            _processor.ProcessErrorAsync += OnErrorMessageAsyc;
            await _processor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }
    }
}
