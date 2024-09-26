using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto> Login(LoginRequestDto loginDto);
        Task<ResponseDto> RegisterUser(RegisterRequestDto registerUserDto);
        Task<ResponseDto> AssignRole(RegisterRequestDto registerUserDto);
    }
}
