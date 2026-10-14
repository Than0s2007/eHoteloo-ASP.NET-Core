// Areas/Admin/Controllers/UsersController.cs
using eHoteloo.Areas.Admin.Models;
using eHoteloo.Data;
using eHoteloo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eHoteloo.Areas.Admin.Controllers
{
    [Area("Admin")]
 //   [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<eHoteloo.Models.User> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<eHoteloo.Models.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==================== INDEX ====================
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var viewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                viewModels.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "User",
                    Roles = roles.ToList(),
                    Balance = user.Balance,
                    IsActive = user.IsActive,
                    IdentityVerified = user.IdentityVerified,
                    EmailConfirmed = user.EmailConfirmed,
                    CreatedAt = user.CreatedAt
                });
            }

            return View(viewModels);
        }

        // ==================== DETAILS ====================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.Users
                .Include(u => u.Bookings)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            bool isEmailVerfird = await _userManager.IsEmailConfirmedAsync(user);
            var vm = new AdminUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "User",
                Roles = roles.ToList(),
                Balance = user.Balance,
                IsActive = user.IsActive,
                IdentityVerified = user.IdentityVerified,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                Bookings = user.Bookings?.ToList() ?? new List<Booking>(),
                EmailVerified = user.EmailVerified
            };

            return View(vm);
        }

        // ==================== EDIT (GET) ====================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault() ?? "User";

            var vm = new AdminUserEditViewModel
            {
                Id = user.Id,
                CurrentRole = currentRole,
                IsActive = user.IsActive,
                IdentityVerified = user.IdentityVerified,
                Balance = user.Balance // 👈 مهم
            };

            return View(vm);
        }

        // ==================== EDIT (POST) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminUserEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            // Update basic fields
            user.IsActive = model.IsActive;
            user.IdentityVerified = model.IdentityVerified;
            user.Balance = model.Balance; // 👈 الجديد
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            // Handle Role change
            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault();

            if (currentRole != model.CurrentRole)
            {
                // Remove old role if exists
                if (!string.IsNullOrEmpty(currentRole))
                {
                    var removeResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                    if (!removeResult.Succeeded)
                    {
                        foreach (var error in removeResult.Errors)
                            ModelState.AddModelError("", error.Description);
                        return View(model);
                    }
                }

                // Add new role
                if (!string.IsNullOrEmpty(model.CurrentRole))
                {
                    // Ensure role exists in system
                    if (!await _userManager.IsInRoleAsync(user, model.CurrentRole))
                    {
                        var addResult = await _userManager.AddToRoleAsync(user, model.CurrentRole);
                        if (!addResult.Succeeded)
                        {
                            foreach (var error in addResult.Errors)
                                ModelState.AddModelError("", error.Description);
                            return View(model);
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // ==================== DELETE (Optional) ====================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}