using eHoteloo.Models;

namespace eHoteloo.VM
{
    public class EditRoomVM
    {
        public string RoomNumber { get; set; }
        public RoomType RoomType { get; set; }
        public string Description { get; set; }
        public decimal PricePerDay { get; set; }
        public bool IsActive { get; set; }

        // بدل Images
        public List<IFormFile>? Images { get; set; }
    }
}
