namespace Mango.Services.EmailAPI.Messaging
{
    public interface IMessaging
    {
        Task Start();
        Task Stop();
    }
}
