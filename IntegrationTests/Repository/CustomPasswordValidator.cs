using System.Collections.Generic;
using System.Threading.Tasks;

using Domain.Models.Database;

using Microsoft.AspNetCore.Identity;

namespace IntegrationTests.Repository
{
    public class CustomPasswordValidator : IPasswordValidator<UserDTO>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<UserDTO> manager, UserDTO user, string password)
        {
            List<IdentityError> errors = new();

            if (password.Length < 6)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordTooShort",
                    Description = "Password should be at least 6 characters long."
                });
            }

            return errors.Count > 0 ? Task.FromResult(IdentityResult.Failed(errors.ToArray())) : Task.FromResult(IdentityResult.Success);
        }
    }

}