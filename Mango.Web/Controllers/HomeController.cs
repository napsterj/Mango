using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        public HomeController(IAuthService authService)
        {

        }

        [HttpGet]
        public IActionResult Index()
        {
            var userClaims = User.Claims;

            if (userClaims == null || !userClaims.Any())
            {
                TempData["Error"] = "Unable to retrieve user data";
                return View();
            }

            var userDto = new UserDto
            {
                Email = userClaims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value,
                UserId = userClaims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value,
                Name = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value,                
                PhoneNumber = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone).Value,
            };

            return View(userDto);
                     
        }       
    }
}
