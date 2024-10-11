using Mango.Services.EmailAPI.Messaging;
using System.Reflection.Metadata;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtension
    {
        public static IMessaging messaging;
        public static IEmailCreationMessage emailCreationMessage;
        public static IApplicationBuilder UseServiceBusConsumer(this IApplicationBuilder app) 
        {
            messaging = app.ApplicationServices.GetRequiredService<IMessaging>();
            emailCreationMessage = app.ApplicationServices.GetRequiredService<IEmailCreationMessage>();
            var appHostLifeTime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            //appHostLifeTime.ApplicationStarted.Register(OnStart);
            //appHostLifeTime.ApplicationStopping.Register(OnStop);
            return app;
        }

        private static void OnStop()
        {
            messaging.Stop();
            emailCreationMessage.Stop();
        }

        private static void OnStart()
        {
            messaging.Start();
            emailCreationMessage.Start();
        }
    }
}
