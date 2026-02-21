using Microsoft.AspNetCore.Identity;

namespace ChucksKitchenApi.Entity
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }
}
