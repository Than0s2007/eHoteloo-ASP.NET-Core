// Models/ViewModels/RoomFilterViewModel.cs
namespace eHoteloo.Models.ViewModels
{
    public class RoomFilterViewModel
    {
        public RoomType? RoomType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Capacity { get; set; }
        public string SortBy { get; set; } = "priceAsc";
    }
}