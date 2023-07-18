using System.Threading.Tasks;

using Domain.Models.Database;

using Microsoft.AspNetCore.Identity;

namespace IntegrationTests.Repository
{
    public class CustomUserValidator : UserValidator<UserDTO>
    {
        private readonly UserManager<UserDTO> _userManager;

        public CustomUserValidator(UserManager<UserDTO> userManager) : base(userManager.ErrorDescriber)
        {
            _userManager = userManager;
        }

        public override async Task<IdentityResult> ValidateAsync(UserManager<UserDTO> manager, UserDTO user)
        {
            IdentityResult result = await base.ValidateAsync(manager, user);

            if (result.Succeeded)
            {
                // Check for duplicate email during registration
                UserDTO existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "DuplicateEmail",
                        Description = $"Email '{user.Email}' is already taken."
                    });
                }
            }

            return result;
        }
    }

}