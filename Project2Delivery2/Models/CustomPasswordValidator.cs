using Microsoft.AspNetCore.Identity;
using Project2Delivery2.DataAccessLayer.Models;

namespace Project2Delivery2.Models
{
    public class CustomPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            var errors = new List<IdentityError>();

            // Custom rule 1: Password must not contain username
            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainsUsername",
                    Description = "Password cannot contain your username"
                });
            }

            // Custom rule 2: Password must not contain email
            if (!string.IsNullOrEmpty(user.Email) && password.ToLower().Contains(user.Email.Split('@')[0].ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainsEmail",
                    Description = "Password cannot contain your email address"
                });
            }

            // Custom rule 3: Password must contain at least 2 digits
            if (password.Count(char.IsDigit) < 2)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordNeedsTwoDigits",
                    Description = "Password must contain at least 2 digits"
                });
            }

            // Custom rule 4: Password must not contain common words
            var commonPasswords = new[] { "password", "123456", "qwerty", "admin", "letmein" };
            if (commonPasswords.Any(cp => password.ToLower().Contains(cp)))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordTooCommon",
                    Description = "Password contains commonly used words"
                });
            }

            return errors.Count == 0
                ? IdentityResult.Success
                : IdentityResult.Failed(errors.ToArray());
        }
    }
}