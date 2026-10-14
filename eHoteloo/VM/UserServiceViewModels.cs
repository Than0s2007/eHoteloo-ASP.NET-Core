using eHoteloo.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace eHoteloo.Areas.User.ViewModels
{
    public class UserServiceListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ServiceStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RoomNumber { get; set; }
        public string BookingNumber { get; set; }
    }

    public class ServiceDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ServiceStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BookingNumber { get; set; }
        public string RoomNumber { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class CreateServiceViewModel
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Service title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [Display(Name = "Service Title")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        [Display(Name = "Details / Notes")]
        public string Description { get; set; }

        [Range(0.01, 100000, ErrorMessage = "Price must be greater than 0.")]
        [Display(Name = "Expected Price")]
        public decimal Price { get; set; }
    }
}