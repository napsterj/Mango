using Mango.MessageBus;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthApiDbContext _authContext;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public AuthService(AuthApiDbContext authContext,
                           SignInManager<AppUser> signInManager,
                           UserManager<AppUser> userManager,
                           RoleManager<IdentityRole> roleManager,
                           IJwtTokenGenerator jwtTokenGenerator,
                           IMessageBus messageBus,
                           IConfiguration configuration)
        {
            _authContext = authContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        public async Task<bool> AssignRole(string email, IEnumerable<string> roles)
        {
            var user = await _authContext.AppUsers.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
            if(user != null) 
            {
                foreach (var role in roles)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(role.ToUpper());
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role.ToUpper()));
                    }
                    await _userManager.AddToRoleAsync(user, role.ToUpper());
                }

                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {

            var user = await _authContext.AppUsers.FirstOrDefaultAsync(usr => usr.UserName.ToLower() == loginRequestDto.Email.ToLower());

            var isValidUser = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            
            if (user == null || !isValidUser) 
            {
                return new LoginResponseDto { UserDto = null,Token = "" };
               
            }
            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user, roles);            

            UserDto userDto = new()
            {
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                UserId = user.Id,
            };

            return new LoginResponseDto { UserDto = userDto, Token = token };                        
        }

        public async Task<string> RegisterUser(RegisterRequestDto registerRequestDto)
        {
            var user = new AppUser
            {
                UserName = registerRequestDto.Email,
                NormalizedEmail = registerRequestDto.Email,
                Name = registerRequestDto.Name,
                Email = registerRequestDto.Email,
                PhoneNumber = registerRequestDto.PhoneNumber,                            
            };

            try
            {
                var identityResult = await _userManager.CreateAsync(user, registerRequestDto.Password);
                
                if (identityResult == null || !identityResult.Succeeded)
                {
                    return identityResult.Errors.Select(e => String.Join(",", e.Description)).ToString();                    
                }

                // Commenting below code to save Azure free credit. Uncomment when using Service bus functionality.
                //var queueName = _configuration.GetSection("ServiceBusSettings").GetValue<string>("QueueName");                
                //await _messageBus.PostMessageToBus(registerRequestDto.Email, queueName);

                return "";                             
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }                        
        }
    }
}
