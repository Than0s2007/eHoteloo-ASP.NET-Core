// Areas/Admin/Controllers/DashboardController.cs
using eHoteloo.Data;
using eHoteloo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eHoteloo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                TotalRooms = await _context.Rooms.CountAsync(),
                TotalBookings = await _context.Bookings.CountAsync(),
                TotalPayments = await _context.Payments.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                TotalServices = await _context.Services.CountAsync(),

                ActiveRooms = await _context.Rooms.CountAsync(r => r.IsActive),
                PendingBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending),
                TotalRevenue = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Success)
                    .SumAsync(p => p.Amount),

                RecentBookings = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Room)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboard);
        }
    }

    // ViewModel للـ Dashboard
    public class DashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int TotalBookings { get; set; }
        public int TotalPayments { get; set; }
        public int TotalUsers { get; set; }
        public int TotalServices { get; set; }

        public int ActiveRooms { get; set; }
        public int PendingBookings { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<Booking> RecentBookings { get; set; }
    }
}