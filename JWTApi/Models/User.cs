
using Microsoft.AspNetCore.Identity;

namespace JWTApi.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
    }
}
