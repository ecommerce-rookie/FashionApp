using System.ComponentModel.DataAnnotations;

namespace IdentityService.Pages.Account.Login
{
    public class RegisterUserModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? ReturnUrl { get; set; }
        public string? Button { get; set; }
    }
}
