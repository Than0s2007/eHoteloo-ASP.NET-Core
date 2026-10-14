// Booking.cs
namespace eHoteloo.Models
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        CheckedIn,
        CheckedOut,
        Cancelled
    }

    public class Booking
    {
        public int Id { get; set; }
        public string BookingNumber { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal BookingPrice { get; set; }  // decimal
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Computed
        public int Days => (CheckOut - CheckIn).Days;

        // Navigation properties
        public User User { get; set; }
        public Room Room { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Service> Services { get; set; }
    }
}