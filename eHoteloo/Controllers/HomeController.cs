// Controllers/HomeController.cs
using eHoteloo.Data;
using eHoteloo.Models;
using eHoteloo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eHoteloo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==================== INDEX (Room Listing) ====================
        public async Task<IActionResult> Index(
            RoomType? roomType,
            decimal? minPrice,
            decimal? maxPrice,
            int? capacity,
            string sortBy = "priceAsc")
        {
            // Base query: active rooms only (eligible for booking)
            var query = _context.Rooms
                .AsNoTracking()
                .Where(r => r.IsActive)
                .Include(r => r.Images)
                .AsQueryable();

            // --- Filtering ---
            if (roomType.HasValue)
            {
                query = query.Where(r => r.RoomType == roomType.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(r => r.PricePerDay >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(r => r.PricePerDay <= maxPrice.Value);
            }

            // Capacity filter (mapped from RoomType for simplicity)
            if (capacity.HasValue)
            {
                query = query.Where(r =>
                    (capacity.Value == 1 && r.RoomType == RoomType.Single) ||
                    (capacity.Value == 2 && r.RoomType == RoomType.Double) ||
                    (capacity.Value >= 3 && r.RoomType == RoomType.Suite));
            }

            // --- Sorting ---
            query = sortBy?.ToLower() switch
            {
                "pricedesc" => query.OrderByDescending(r => r.PricePerDay),
                "type" => query.OrderBy(r => r.RoomType),
                _ => query.OrderBy(r => r.PricePerDay) // priceAsc default
            };

            var rooms = await query.ToListAsync();

            // Build ViewModel
            var viewModel = new RoomListViewModel
            {
                Rooms = rooms,
                Filter = new RoomFilterViewModel
                {
                    RoomType = roomType,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Capacity = capacity,
                    SortBy = sortBy
                }
            };

            return View(viewModel);
        }

        // ==================== DETAILS (Room Details) ====================
        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms
                .AsNoTracking()
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // ==================== BOOK NOW (POST) ====================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookNow(int roomId, DateTime checkIn, DateTime checkOut)
        {
            // Validate dates
            if (checkIn >= checkOut)
            {
                TempData["Error"] = "Check-out date must be after check-in date.";
                return RedirectToAction("Details", new { id = roomId });
            }

            if (checkIn < DateTime.UtcNow.Date)
            {
                TempData["Error"] = "Check-in date cannot be in the past.";
                return RedirectToAction("Details", new { id = roomId });
            }

            // Get room
            var room = await _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roomId && r.IsActive);

            if (room == null)
            {
                TempData["Error"] = "Room not found or not available.";
                return RedirectToAction("Index");
            }

            // Get current user
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Unauthorized();
            }

            // Calculate total price
            int days = (checkOut - checkIn).Days;
            decimal totalPrice = room.PricePerDay * days;

            // Check user balance
            if (user.Balance < totalPrice)
            {
                TempData["Error"] = $"Insufficient balance. Required: {totalPrice:C}, Available: {user.Balance:C}";
                return RedirectToAction("Details", new { id = roomId });
            }

            // Check for overlapping bookings
            bool hasOverlap = await _context.Bookings
                .AnyAsync(b =>
                    b.RoomId == roomId &&
                    b.Status != BookingStatus.Cancelled &&
                    checkIn < b.CheckOut &&
                    checkOut > b.CheckIn);

            if (hasOverlap)
            {
                TempData["Error"] = "This room is not available for the selected dates.";
                return RedirectToAction("Details", new { id = roomId });
            }

            // Create booking
            var booking = new Booking
            {
                BookingNumber = "BK-" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
                UserId = userId,
                RoomId = roomId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                BookingPrice = totalPrice,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            // Deduct from balance
            user.Balance -= totalPrice;

            // Create payment record
            var payment = new Payment
            {
                BookingId = 0, // Will be set after booking save, handled by EF navigation
                Amount = totalPrice,
                Method = PaymentMethod.OnlinePayment,
                Status = PaymentStatus.Success,
                PaidAt = DateTime.UtcNow
            };

            // Use transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Link payment to booking
                payment.BookingId = booking.Id;
                _context.Payments.Add(payment);

                // Update user balance
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Failed to update user balance.";
                    return RedirectToAction("Details", new { id = roomId });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Booking confirmed! Booking number: {booking.BookingNumber}. Total paid: {totalPrice:C}";
                return RedirectToAction("BookingConfirmation", new { id = booking.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "An error occurred while processing your booking. Please try again.";
                return RedirectToAction("Details", new { id = roomId });
            }
        }

        // ==================== BOOKING CONFIRMATION ====================
        [Authorize]
        public async Task<IActionResult> BookingConfirmation(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Room)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
    }
}