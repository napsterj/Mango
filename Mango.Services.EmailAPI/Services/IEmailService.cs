using Mango.Services.Email.API.Models.Dto;

namespace Mango.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCart(CartDto cartDto);
        Task LogNewCreatedUserEmail(string email);
    }
}
