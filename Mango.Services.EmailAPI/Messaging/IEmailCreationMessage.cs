namespace Mango.Services.EmailAPI.Messaging
{
    public interface IEmailCreationMessage
    {
        Task Start();
        Task Stop();
    }
}
