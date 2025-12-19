using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace HabitGoalTrackerApp.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailSender _emailSender;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthController> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        #region Login

        // GET: /auth/login
        [Route("auth/login")]
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            // If user is already authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /auth/login
        [Route("auth/login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            model.ReturnUrl ??= Url.Content("~/dashboard");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Update last login time
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        user.LastLoginAt = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                    }

                    return LocalRedirect(model.ReturnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    // Redirect to two-factor authentication page if needed
                    return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError(string.Empty, "This account has been locked out. Please try again later.");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Register

        // GET: /auth/register
        [Route("auth/register")]
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            // If user is already authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        // POST: /auth/register
        [Route("auth/register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            model.ReturnUrl ??= Url.Content("~/Dashboard");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = false, // Require email confirmation
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Generate email confirmation token
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Auth",
                        new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        model.Email,
                        "Confirm your email - Streakly",
                        $@"<h2>Welcome to Streakly!</h2>
                        <p>Thank you for registering. Please confirm your email address by clicking the link below:</p>
                        <p><a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>Confirm Email Address</a></p>
                        <p>If you didn't create an account, you can safely ignore this email.</p>
                        <br/>
                        <p>Best regards,<br/>The Streakly Team</p>");

                    return RedirectToAction(nameof(RegisterConfirmation), new { email = model.Email });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /auth/register-confirmation
        [Route("auth/register-confirmation")]
        [HttpGet]
        public IActionResult RegisterConfirmation(string email)
        {
            ViewData["Email"] = email;
            return View();
        }

        // GET: /auth/confirm-email
        [Route("auth/confirm-email")]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                _logger.LogInformation("User confirmed their email successfully.");
                TempData["SuccessMessage"] = "Thank you for confirming your email. You can now log in.";
                return View("ConfirmEmailSuccess");
            }
            else
            {
                _logger.LogWarning("Error confirming email for user {UserId}.", userId);
                return View("ConfirmEmailError");
            }
        }

        #endregion

        #region Logout

        // POST: /auth/logout
        [Route("auth/logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region Forgot Password

        // GET: /auth/forgot-password
        [Route("auth/forgot-password")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /auth/forgot-password
        [Route("auth/forgot-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                // Don't reveal that the user does not exist or is not confirmed
                // Always show the confirmation page for security reasons
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // Generate password reset token
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Auth",
                    new { code },
                    protocol: Request.Scheme);

                try
                {
                    await _emailSender.SendEmailAsync(
                        model.Email,
                        "Reset Password",
                        $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

                    _logger.LogInformation("Password reset email sent to {Email}", model.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending password reset email to {Email}", model.Email);
                    // Still redirect to confirmation for security, but log the error
                }

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }

        // GET: /auth/forgot-password-confirmation
        [Route("auth/forgot-password-confirmation")]
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Reset Password

        // GET: /auth/reset-password
        [Route("auth/reset-password")]
        [HttpGet]
        public IActionResult ResetPassword(string? code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }

            // Keep the code in its encoded form
            var model = new ResetPasswordViewModel
            {
                Code = code
            };

            return View(model);
        }

        // POST: /auth/reset-password
        [Route("auth/reset-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            // Decode the token before passing to ResetPasswordAsync
            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ResetPasswordAsync(user, decodedCode, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successful for user {Email}", model.Email);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /auth/reset-password-confirmation
        [Route("auth/reset-password-confirmation")]
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Access Denied

        // GET: /auth/access-denied
        [Route("auth/access-denied")]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region Two-Factor Authentication (Optional Placeholder)

        // GET: /auth/login-with-2fa
        [Route("auth/login-with-2fa")]
        [HttpGet]
        public IActionResult LoginWith2fa(bool rememberMe, string? returnUrl = null)
        {
            // Placeholder for two-factor authentication
            // Implement if needed
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["RememberMe"] = rememberMe;
            return View();
        }

        #endregion
    }
}
