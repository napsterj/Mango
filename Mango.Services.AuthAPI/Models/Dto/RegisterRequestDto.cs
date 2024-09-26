using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthAPI.Models.Dto
{
    public class RegisterRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
       
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        
        public List<RolesDto>? Roles { get; set; }
        public List<string> MultiRoles { get; set; }
     }
}
