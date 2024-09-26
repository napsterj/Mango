using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Services.IService
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<string> RegisterUser(RegisterRequestDto registerRequestDto);

        Task<bool> AssignRole(string email, IEnumerable<string> roles);
    }
}
