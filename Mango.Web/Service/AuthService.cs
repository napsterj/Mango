using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using static Mango.Web.Utilities.Enums;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> AssignRole(RegisterRequestDto registerUserDto)
        {
            RequestDto requestDto = new()
            {
                ApiType = ApiType.POST,
                Data = registerUserDto,
                Url = $"{Enums.AuthAPIBase}/api/auth/AssignRole"
            };

            return await _baseService.SendAsync(requestDto);
        }

        public Task<ResponseDto> Login(LoginRequestDto loginDto)
        {
            RequestDto requestDto = new()
            {
                ApiType = ApiType.POST,                
                Data = loginDto,
                Url = $"{AuthAPIBase}/api/auth/login"
            };

            return _baseService.SendAsync(requestDto);
        }

        public Task<ResponseDto> RegisterUser(RegisterRequestDto registerUserDto)
        {
            RequestDto requestDto = new()
            {
                ApiType = ApiType.POST,                
                Data = registerUserDto,
                Url = $"{AuthAPIBase}/api/auth/Register"
            };

            return _baseService.SendAsync(requestDto);
        }
    }
}
