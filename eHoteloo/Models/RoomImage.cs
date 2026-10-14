// RoomImages.cs
namespace eHoteloo.Models
{
    public class RoomImage  
    {
        public int Id { get; set; }
        public int RoomId { get; set; }  
        public string ImagePath { get; set; }  
        // Navigation
        public Room Room { get; set; }
    }
}