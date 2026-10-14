using eHoteloo.Data;
using eHoteloo.Services;
using eHoteloo.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eHoteloo.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<eHoteloo.Models.User> _userManager;
        private readonly SignInManager<eHoteloo.Models.User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;


        public HomeController(UserManager<Models.User> userManager, SignInManager<Models.User> signInManager, ApplicationDbContext context, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            eHoteloo.Models.User MyUser = await _userManager.FindByIdAsync(userId);
            if (MyUser is null)
            {
                return NotFound();
            }
            UserProfileVM model = new UserProfileVM
            {
                FirstName = MyUser.FirstName,
                LastName = MyUser.LastName,
                Email = MyUser.Email,
                PhoneNumber = MyUser.PhoneNumber,
                IdentityPhotoName = MyUser.IdentityPhotoName,
                IdentityVerified = MyUser.IdentityVerified,
                Balance = MyUser.Balance,
            };
            return View(model);

        }
        // GET: User/Home/BookingHistory
        public async Task<IActionResult> BookingHistory()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var bookings = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .Include(b => b.Room)
                    .ThenInclude(r => r.Images)
                .Include(b => b.Payments)
                .Include(b => b.Services)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bookings);
        }

        // GET: User/Home/BookingDetails/5
        public async Task<IActionResult> BookingDetails(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Room)
                    .ThenInclude(r => r.Images)
                .Include(b => b.Payments)
                .Include(b => b.Services)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
        // Get User/Home/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(User);
            eHoteloo.Models.User MyUser = await _userManager.FindByIdAsync(userId);
            if (MyUser is null)
            {
                return NotFound();
            }
            var model = new
            {
                FirstName = MyUser.FirstName,
                LastName = MyUser.LastName,
                IdentityPhotoName = MyUser.IdentityPhotoName
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string FirstName, string LastName, IFormFile? IdentityPhoto)
        {
            var userId = _userManager.GetUserId(User);
            var myUser = await _userManager.FindByIdAsync(userId);

            if (myUser == null)
                return NotFound();

            myUser.FirstName = FirstName;
            myUser.LastName = LastName;

            if (IdentityPhoto != null && IdentityPhoto.Length > 0)
            {
                // اسم جديد عشان ميحصلش تكرار
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(IdentityPhoto.FileName);

                // المسار
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/IdentityImage");

                // تأكد إن الفولدر موجود
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, fileName);

                // حفظ الصورة
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await IdentityPhoto.CopyToAsync(stream);
                }

                // (اختياري 🔥) امسح الصورة القديمة
                if (!string.IsNullOrEmpty(myUser.IdentityPhotoName))
                {
                    var oldPath = Path.Combine(folderPath, myUser.IdentityPhotoName);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // خزّن الاسم في الداتابيز
                myUser.IdentityPhotoName = fileName;
            }

            await _userManager.UpdateAsync(myUser);

            return RedirectToAction("Index");
        }
        // Change Email

        public async Task<IActionResult> ChangeEmail()
        {
            var userId = _userManager.GetUserId(User);
            var myUser = await _userManager.FindByIdAsync(userId);

            return View((object)myUser.Email);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(string email)
        {
            var userId = _userManager.GetUserId(User);
            var myUser = await _userManager.FindByIdAsync(userId);

            if (myUser == null)
                return NotFound();

            if (string.IsNullOrEmpty(email))
                return BadRequest();

            if (email == myUser.Email)
                return View();


            var token = await _userManager.GenerateChangeEmailTokenAsync(myUser, email);


            // link
            var confirmationLink = Url.Action(
                "ConfirmChangeEmail",
                "Home",
                new { userId = myUser.Id, email = email, token = token },
                protocol: HttpContext.Request.Scheme);

            // ✅ Send Email
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your new email - eHoteloo",
                $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;padding:40px 20px;text-align:center;'>
            <h2 style='color:#3b6ff0;'>Confirm your new email</h2>
            <p style='color:#666;font-size:16px;'>Click the button below to confirm your new email address.</p>
            <a href='{confirmationLink}' style='display:inline-block;padding:14px 32px;background:#3b6ff0;color:#fff;text-decoration:none;border-radius:8px;font-weight:600;margin:20px 0;'>Confirm Email</a>
            <p style='color:#999;font-size:13px;'>Or copy this link:<br/>{confirmationLink}</p>
        </div>");

            return RedirectToAction("index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmChangeEmail(int userId, string email, string token)
        {
            if (userId == 0 || email == null || token == null)
            {
                TempData["Error"] = "Invalid confirmation link.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ChangeEmailAsync(user, email, token);

            if (result.Succeeded)
            {
                user.UserName = email;
                await _userManager.UpdateAsync(user);

                TempData["Success"] = "Your email has been successfully changed and verified!";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Email confirmation failed. The link may be expired or invalid.";
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword() 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var userId = _userManager.GetUserId(User);
            var myUser = await _userManager.FindByIdAsync(userId);

            if (myUser == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(myUser, oldPassword, newPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(myUser);

                return RedirectToAction("Index"); 
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount() 
        {
            var userId = _userManager.GetUserId(User);
            var myUser = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.DeleteAsync(myUser);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            // لو فشل
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}