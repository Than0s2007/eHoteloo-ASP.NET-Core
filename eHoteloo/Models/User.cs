// Models/User.cs
using Microsoft.AspNetCore.Identity;

namespace eHoteloo.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? IdentityPhotoName { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IdentityVerified { get; set; } = false;
        public bool EmailVerified { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; }
    }
}