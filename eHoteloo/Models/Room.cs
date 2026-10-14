// Rooms.cs
namespace eHoteloo.Models
{
    public enum RoomType
    {
        Single,
        Double,
        Suite
    }

    public class Room  
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public RoomType RoomType { get; set; }
        public string Description { get; set; }
        public decimal PricePerDay { get; set; }  
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<RoomImage> Images { get; set; }  
    }
}