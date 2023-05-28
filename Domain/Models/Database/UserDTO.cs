using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Database
{
    public class UserDTO : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}