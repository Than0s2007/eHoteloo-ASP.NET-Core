// Areas/Admin/Models/AdminUserViewModel.cs
using eHoteloo.Models;

namespace eHoteloo.Areas.Admin.Models
{
    public class AdminUserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }          // الدور الأساسي (أول دور)
        public List<string> Roles { get; set; }   // كل الأدوار
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public bool IdentityVerified { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Booking> Bookings { get; set; } = new();
        public bool EmailVerified { get; set; }
    }

    // للـ Edit عشان نملى الـ Dropdown بالأدوار المتاحة
    public class AdminUserEditViewModel
    {
        public int Id { get; set; }
        public string CurrentRole { get; set; }
        public List<string> AvailableRoles { get; set; } = new() { "User", "Admin", "Staff" };
        public bool IsActive { get; set; }
        public bool IdentityVerified { get; set; }
        public decimal Balance { get; set; }
    }
}