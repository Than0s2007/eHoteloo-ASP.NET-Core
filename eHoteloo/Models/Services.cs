// Services.cs
namespace eHoteloo.Models
{
    public enum ServiceStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public class Service  
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }  

        public int? PaymentId { get; set; }  
        public Payment Payment { get; set; }

        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public Booking Booking { get; set; }
       
    }
}