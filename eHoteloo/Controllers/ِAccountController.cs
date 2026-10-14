// Controllers/AccountController.cs
using eHoteloo.Models;
using eHoteloo.Models.ViewModels;
using eHoteloo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eHoteloo.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // ==================== REGISTER ====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add to User role
                    await _userManager.AddToRoleAsync(user, "User");

                    // Generate email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token = token },
                        protocol: HttpContext.Request.Scheme);

                    // Send email
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Confirm your email - eHoteloo",
                        $@"
                        <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;padding:40px 20px;text-align:center;'>
                            <h2 style='color:#3b6ff0;'>Welcome to eHoteloo!</h2>
                            <p style='color:#666;font-size:16px;'>Please confirm your email address to complete your registration.</p>
                            <a href='{confirmationLink}' style='display:inline-block;padding:14px 32px;background:#3b6ff0;color:#fff;text-decoration:none;border-radius:8px;font-weight:600;margin:20px 0;'>Confirm Email</a>
                            <p style='color:#999;font-size:13px;'>Or copy this link:<br/>{confirmationLink}</p>
                        </div>");

                    return RedirectToAction("RegisterConfirmation");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation() => View();

        // ==================== CONFIRM EMAIL ====================
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            if (userId == 0 || token == null)
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return View("ConfirmEmailSuccess");

            return View("Error");
        }

        // ==================== LOGIN ====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Please confirm your email before logging in.");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your account has been deactivated. Contact support.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            return View(model);
        }


        // ==================== FORGOT PASSWORD ====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { email = user.Email, token = token },
                    protocol: HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Reset your password - eHoteloo",
                    $@"
                    <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;padding:40px 20px;text-align:center;'>
                        <h2 style='color:#3b6ff0;'>Password Reset</h2>
                        <p style='color:#666;font-size:16px;'>Click the button below to reset your password.</p>
                        <a href='{resetLink}' style='display:inline-block;padding:14px 32px;background:#3b6ff0;color:#fff;text-decoration:none;border-radius:8px;font-weight:600;margin:20px 0;'>Reset Password</a>
                        <p style='color:#999;font-size:13px;'>Or copy this link:<br/>{resetLink}</p>
                        <p style='color:#999;font-size:12px;margin-top:30px;'>If you didn't request this, please ignore this email.</p>
                    </div>");

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation() => View();

        // ==================== RESET PASSWORD ====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
                return RedirectToAction("Index", "Home");

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation() => View();

        // ==================== ACCESS DENIED ====================
        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}