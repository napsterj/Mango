using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new();            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequestDto loginRequestDto)
        {                        
            var response = await _authService.Login(loginRequestDto);
            
            if(response == null || response.UserDto == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Email or password is incorrect";
                return BadRequest(_responseDto);
            }

            _responseDto.IsSuccess = true;
            _responseDto.Result = response;

            return Ok(JsonConvert.SerializeObject(_responseDto));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody]RegisterRequestDto registerRequestDto)
        {
            var response = await _authService.RegisterUser(registerRequestDto);
            
            if (!string.IsNullOrEmpty(response))
            {
                _responseDto.Message = response;
                _responseDto.IsSuccess = false;

                return BadRequest(_responseDto);                
            }
            
            _responseDto.IsSuccess = true;

            return Ok(_responseDto);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] RegisterRequestDto registerRequestDto)
        {
            var areRolesAssigned = await _authService.AssignRole(registerRequestDto.Email, registerRequestDto.MultiRoles);
            
            if (!areRolesAssigned)
            {
                _responseDto.IsSuccess = false; 
                _responseDto.Message = "Error Encountered";
            }

            _responseDto.IsSuccess = true;
            return Ok(_responseDto);
        }

        
    }
}
