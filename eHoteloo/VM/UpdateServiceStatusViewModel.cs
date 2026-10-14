using eHoteloo.Models;
using System.ComponentModel.DataAnnotations;

namespace eHoteloo.Areas.Staff.ViewModels
{
    public class UpdateServiceStatusViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Guest Name")]
        public string GuestName { get; set; }

        [Display(Name = "Service Name")]
        public string ServiceName { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Please select a status.")]
        [Display(Name = "Current Status")]
        public ServiceStatus CurrentStatus { get; set; }
    }
}