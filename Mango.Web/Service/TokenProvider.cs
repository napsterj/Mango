using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(Enums.JwtCookie); 
        }

        public string? GetToken()
        {
            string? token = "";
            if (_contextAccessor.HttpContext?.Request.Cookies.Count > 0)
            {
                _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(Enums.JwtCookie, out token);
            }
            return token;
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(Enums.JwtCookie, token);
        }
    }
}
