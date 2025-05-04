using IdentityService.Enums;
using Microsoft.AspNetCore.Identity;

namespace EcommerceApp.IdentityService.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Status { get; set; }
    }
}
