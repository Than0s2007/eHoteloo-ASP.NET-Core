using eHoteloo.Models;

public class ServiceRequestViewModel
{
    public int Id { get; set; }
    public string GuestName { get; set; }
    public string ServiceName { get; set; }
    public DateTime RequestDate { get; set; }
    public ServiceStatus CurrentStatus { get; set; }
    public string Notes { get; set; }
    public string BookingNumber { get; set; }
    public string RoomNumber { get; set; }
    public string GuestPhone { get; set; } // ← جديد
}