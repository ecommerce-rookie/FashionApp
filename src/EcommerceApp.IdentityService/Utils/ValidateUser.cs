using IdentityService.Pages.Account.Login;

namespace IdentityService.Utils
{
    public class ValidateUser
    {
        public static IDictionary<string, string> ValidateRegister(RegisterUserModel registerUserModel)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(registerUserModel.LastName))
            {
                result.Add(nameof(registerUserModel.LastName), "Last name is required.");
            }
            if (string.IsNullOrEmpty(registerUserModel.Email))
            {
                result.Add(nameof(registerUserModel.Email), "Email is required.");
            } else if (!IsValidEmail(registerUserModel.Email))
            {
                result.Add(nameof(registerUserModel.Email), "Invalid email format.");
            }
            if (string.IsNullOrEmpty(registerUserModel.Password))
            {
                result.Add(nameof(registerUserModel.Password), "Password is required.");
            }
            if (registerUserModel.Password != registerUserModel.ConfirmPassword)
            {
                result.Add(nameof(registerUserModel.ConfirmPassword), "Password and confirm password do not match.");
            }

            return result;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);

                return addr.Address.Equals(email);
            } catch
            {
                return false;
            }
        }

    }
}
