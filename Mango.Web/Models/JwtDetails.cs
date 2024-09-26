namespace Mango.Web.Models
{
    public class JwtDetails
    {
        public string Email { get; set; }
        public HashSet<string> Roles { get; set; }
        public string Name {  get; set; }

        public string UserId { get; set; }
    }
}
