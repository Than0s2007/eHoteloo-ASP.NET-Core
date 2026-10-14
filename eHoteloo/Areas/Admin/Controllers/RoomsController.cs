// Areas/Admin/Controllers/RoomsController.cs
using eHoteloo.Data;
using eHoteloo.Models;
using eHoteloo.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eHoteloo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Rooms
                .Include(r => r.Images)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Images)
                .Include(r => r.Bookings)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return NotFound();

            return View(room);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomCreateVM vm)
        {
            if (ModelState.IsValid)
            {
                var room = new Room
                {
                    RoomNumber = vm.RoomNumber,
                    RoomType = vm.RoomType,
                    Description = vm.Description,
                    PricePerDay = vm.PricePerDay,
                    IsActive = vm.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow
                };

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                // رفع الصور
                if (vm.Images != null && vm.Images.Any())
                {
                    foreach (var file in vm.Images)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var path = Path.Combine("wwwroot/images/rooms", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new RoomImage
                        {
                            ImagePath = "/images/rooms/" + fileName,
                            RoomId = room.Id
                        };

                        _context.RoomImages.Add(image);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var vm = new EditRoomVM
            {
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                Description = room.Description,
                PricePerDay = room.PricePerDay,
                IsActive = room.IsActive
            };

            return View(vm); // ✅ كده صح
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditRoomVM vm)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // تحديث البيانات
                    room.RoomNumber = vm.RoomNumber;
                    room.RoomType = vm.RoomType;
                    room.Description = vm.Description;
                    room.PricePerDay = vm.PricePerDay;
                    room.IsActive = vm.IsActive;
                    room.LastUpdatedAt = DateTime.UtcNow;
                    // حذف الصور القديمة
                    var oldImages = _context.RoomImages.Where(i => i.RoomId == room.Id);

                    foreach (var img in oldImages)
                    {
                        var fullPath = Path.Combine("wwwroot", img.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }

                    _context.RoomImages.RemoveRange(oldImages);

                    if (vm.Images != null && vm.Images.Any())
                    {
                        foreach (var file in vm.Images)
                        {
                            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            var path = Path.Combine("wwwroot/images/rooms", fileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var image = new RoomImage
                            {
                                ImagePath = "/images/rooms/" + fileName,
                                RoomId = room.Id
                            };

                            _context.RoomImages.Add(image);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Rooms.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return NotFound();

            if (room.Bookings.Any())
            {
                TempData["Error"] = "Cannot delete room with existing bookings!";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id) => _context.Rooms.Any(e => e.Id == id);
    }
}