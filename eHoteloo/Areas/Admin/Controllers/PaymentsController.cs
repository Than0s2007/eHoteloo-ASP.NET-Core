// Areas/Admin/Controllers/PaymentsController.cs
using eHoteloo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eHoteloo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Services)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();

            return View(payments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Services)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null) return NotFound();

            return View(payment);
        }
    }
}