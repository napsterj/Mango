using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Services.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(AppUser appUser, IEnumerable<string> roles);
        void ClearToken();
    }
}
