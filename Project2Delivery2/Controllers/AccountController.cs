using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.Models;

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
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
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
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Account locked due to too many failed login attempts. Please try again in 5 minutes.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }

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
    }
}