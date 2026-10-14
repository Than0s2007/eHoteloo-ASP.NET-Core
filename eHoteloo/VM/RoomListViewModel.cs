// Models/ViewModels/RoomListViewModel.cs
namespace eHoteloo.Models.ViewModels
{
    public class RoomListViewModel
    {
        public List<Room> Rooms { get; set; } = new List<Room>();
        public RoomFilterViewModel Filter { get; set; } = new RoomFilterViewModel();
    }
}