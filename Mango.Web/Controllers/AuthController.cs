using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        private ResponseDto _responseDto;

        public AuthController(IAuthService authService,
                              ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
            _responseDto = new ResponseDto();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginDto)
        {
            if (ModelState.IsValid)
            {
                _responseDto = await _authService.Login(loginDto);
                
                if(_responseDto != null && !string.IsNullOrWhiteSpace(_responseDto.Message))
                {
                    TempData["Error"] = _responseDto.Message;
                    return View(loginDto);
                }

                var result = JsonConvert.DeserializeObject<DeserializeHandlerForLogin>(Convert.ToString(_responseDto.Result));
                
                var loginResponseDto = result.Result;
                
                _tokenProvider.SetToken(loginResponseDto.Token);

                await SignInUser(loginResponseDto);

                return RedirectToAction("Index", "Home");
            }       
            
            return View(loginDto);
        }

        [HttpGet]
        public IActionResult RegisterUser()
        {
            RegisterRequestDto registerUserDto = new();
            registerUserDto.Roles = PopulateRoles();
                                                                     
            return View(registerUserDto);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterRequestDto registerUserDto)
        {
            if(ModelState.IsValid)
            {
                _responseDto = await _authService.RegisterUser(registerUserDto);

                ResponseDto assignRole;

                if (_responseDto != null && _responseDto.IsSuccess)
                {
                    if (!registerUserDto.MultiRoles.Any())
                    {
                        registerUserDto.MultiRoles.Add("Guest");
                    }

                    assignRole = await _authService.AssignRole(registerUserDto);
                    if (!assignRole.IsSuccess)
                    {
                        TempData["Error"] = assignRole.Message;
                    }

                    TempData["Success"] = "User is successfully registered.";
                    
                    return RedirectToAction("Login");
                }
            }
            
            ModelState.AddModelError("Errors", _responseDto.Message);

            registerUserDto.Roles = PopulateRoles();
            
            return View(registerUserDto);
        }

        [HttpGet]
        public async Task<IActionResult> Signout()
        {
            _tokenProvider.ClearToken();     
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt =  tokenHandler.ReadJwtToken(loginResponseDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
            identity.AddClaim(new Claim(ClaimTypes.MobilePhone, jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.MobilePhone).Value));

            var principal = new ClaimsPrincipal(identity);            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);

        }

        private static List<SelectListItem> PopulateRoles()
        {
            return new()
            {
                new SelectListItem("SuperAdmin", "SuperAdmin"),
                new SelectListItem("Admin", "Admin"),
                new SelectListItem("User", "User"),
                new SelectListItem("Guest", "Guest"),
            };
        }
    }
}
