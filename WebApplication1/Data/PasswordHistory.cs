using Microsoft.AspNetCore.Identity;

namespace eCoinAccountingApp.Data
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string HashedPassword { get; set; }
        public DateTime ChangedDate { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

}
