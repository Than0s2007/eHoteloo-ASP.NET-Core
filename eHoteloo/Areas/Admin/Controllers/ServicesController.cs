// Areas/Admin/Controllers/ServicesController.cs
using eHoteloo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eHoteloo.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .Include(s => s.Booking)
                    .ThenInclude(b => b.User)
                .Include(s => s.Payment)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View(services);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services
                .Include(s => s.Booking)
                    .ThenInclude(b => b.User)
                .Include(s => s.Payment)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null) return NotFound();

            return View(service);
        }
    }
}