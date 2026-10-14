using eHoteloo.Areas.Staff.ViewModels;
using eHoteloo.Data;
using eHoteloo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace eHoteloo.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Admin,Staff")]
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Staff/Services
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .AsNoTracking()
                .Include(s => s.Booking)
                    .ThenInclude(b => b.User)
                .Include(s => s.Booking)
                    .ThenInclude(b => b.Room)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new ServiceRequestViewModel
                {
                    Id = s.Id,
                    GuestName = s.Booking.User != null
                        ? $"{s.Booking.User.FirstName} {s.Booking.User.LastName}"
                        : "Unknown",
                    GuestPhone = s.Booking.User != null ? s.Booking.User.PhoneNumber : "-",
                    ServiceName = s.Title,
                    RequestDate = s.CreatedAt,
                    CurrentStatus = s.Status,
                    Notes = s.Description ?? "-",
                    BookingNumber = s.Booking.BookingNumber,
                    RoomNumber = s.Booking.Room != null ? s.Booking.Room.RoomNumber : "-"
                })
                .ToListAsync();

            return View(services);
        }

        // GET: /Staff/Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var service = await _context.Services
                .Include(s => s.Booking)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound();

            var viewModel = new UpdateServiceStatusViewModel
            {
                Id = service.Id,
                GuestName = service.Booking.User != null
                    ? $"{service.Booking.User.FirstName} {service.Booking.User.LastName}"
                    : "Unknown",
                ServiceName = service.Title,
                Notes = service.Description,
                CurrentStatus = service.Status
            };

            return View(viewModel);
        }

        // POST: /Staff/Services/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateServiceStatusViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(viewModel);

            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            service.Status = viewModel.CurrentStatus;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Service status updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}