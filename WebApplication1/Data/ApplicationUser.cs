using Microsoft.AspNetCore.Identity;

namespace eCoinAccountingApp.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public DateTime? PasswordExpirationDate { get; set; }
        public string Role {  get; set; }
    }
}
