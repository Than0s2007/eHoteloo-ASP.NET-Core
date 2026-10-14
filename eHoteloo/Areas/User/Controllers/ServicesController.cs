using eHoteloo.Areas.User.ViewModels;
using eHoteloo.Data;
using eHoteloo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eHoteloo.Areas.User.Controllers
{
    [Area("User")]
    [Authorize] // أي user مسجل يقدر يدخل
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.User> _userManager;

        public ServicesController(ApplicationDbContext context, UserManager<Models.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Booking الحالية: CheckIn <= Now <= CheckOut ولسه مش Cancelled
        /// </summary>
        private async Task<Booking> GetCurrentBookingAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Room)
                .Where(b => b.UserId == userId &&
                            b.Status != BookingStatus.Cancelled &&
                            b.CheckIn <= now &&
                            b.CheckOut >= now)
                .OrderByDescending(b => b.CheckIn)
                .FirstOrDefaultAsync();
        }

        // GET: /User/Services
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(_userManager.GetUserId(User));
            var currentBooking = await GetCurrentBookingAsync(userId);

            if (currentBooking == null)
            {
                ViewBag.Message = "You don't have an active booking currently.";
                ViewBag.BookingNumber = null;
                return View(new List<UserServiceListViewModel>());
            }

            var services = await _context.Services
                .AsNoTracking()
                .Where(s => s.BookingId == currentBooking.Id)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new UserServiceListViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    Price = s.Price,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    RoomNumber = currentBooking.Room.RoomNumber,
                    BookingNumber = currentBooking.BookingNumber
                })
                .ToListAsync();

            ViewBag.BookingNumber = currentBooking.BookingNumber;
            ViewBag.RoomNumber = currentBooking.Room.RoomNumber;

            return View(services);
        }

        // GET: /User/Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = int.Parse(_userManager.GetUserId(User));

            var service = await _context.Services
                .AsNoTracking()
                .Include(s => s.Booking)
                    .ThenInclude(b => b.Room)
                .Include(s => s.Payment)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound();

            // Security: Service لازم يكون للـ User الحالي (أو Admin)
            if (service.Booking.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var vm = new ServiceDetailsViewModel
            {
                Id = service.Id,
                Title = service.Title,
                Description = service.Description,
                Price = service.Price,
                Status = service.Status,
                CreatedAt = service.CreatedAt,
                BookingNumber = service.Booking.BookingNumber,
                RoomNumber = service.Booking.Room.RoomNumber,
                PaymentStatus = service.Payment != null ? service.Payment.Status.ToString() : "Not Paid"
            };

            return View(vm);
        }

        // GET: /User/Services/Create
        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(_userManager.GetUserId(User));
            var currentBooking = await GetCurrentBookingAsync(userId);

            if (currentBooking == null)
            {
                TempData["ErrorMessage"] = "You must have an active booking to request a service.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new CreateServiceViewModel
            {
                BookingId = currentBooking.Id
            };

            return View(vm);
        }

        // POST: /User/Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceViewModel vm)
        {
            var userId = int.Parse(_userManager.GetUserId(User));

            // Verify booking belongs to current user
            var booking = await _context.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == vm.BookingId && b.UserId == userId);

            if (booking == null)
                return BadRequest("Invalid booking.");

            if (!ModelState.IsValid)
                return View(vm);

            var service = new Service
            {
                BookingId = vm.BookingId,
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                Status = ServiceStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Service request submitted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}