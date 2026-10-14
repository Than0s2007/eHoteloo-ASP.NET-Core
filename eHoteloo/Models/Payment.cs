// Payments.cs
namespace eHoteloo.Models
{
    public enum PaymentMethod
    {
        Cash,
        OnlinePayment
    }

    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed
    }

    public class Payment  
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }  
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime PaidAt { get; set; }

        // Navigation
        public Booking Booking { get; set; }
        public ICollection<Service> Services { get; set; }  
    }
}