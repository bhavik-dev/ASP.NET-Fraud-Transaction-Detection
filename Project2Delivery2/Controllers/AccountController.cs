using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.Models;
using System.Security.Claims;

namespace Project2Delivery2.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FullName = model.FullName,
                    Gender = model.Gender,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    DateOfBirth = model.DateOfBirth,
                    RegisteredAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["SuccessMessage"] = "Registration successful! Welcome " + user.FullName;
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: Account/Login
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Account/Login - SECURE VERSION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Username,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Welcome back, " + model.Username + "!";

                    // SECURE: Only redirect to LOCAL URLs
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Account locked. Try again in 5 minutes.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "You have been logged out successfully!";
            return RedirectToAction("Index", "Home");
        }

        // POST: Account/ExternalLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        // GET: Account/ExternalLoginCallback
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in with external provider
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Welcome back!";
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked.");
                return RedirectToAction("Login");
            }
            else
            {
                // User doesn't exist, create account
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name); 

                if (email != null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = name ?? email,
                        EmailConfirmed = true,
                        RegisteredAt = DateTime.Now
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (createResult.Succeeded)
                    {
                        createResult = await _userManager.AddLoginAsync(user, info);
                        if (createResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            TempData["SuccessMessage"] = "Account created with Google!";
                            return RedirectToLocal(returnUrl);
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Error creating account from external login.");
                return RedirectToAction("Login");
            }
        }

        // Helper method
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}