using Microsoft.AspNetCore.Mvc;

namespace Project2Delivery2.Controllers
{
    public class SecurityDemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Demonstration of VULNERABLE redirect (for educational purposes only)
        public IActionResult VulnerableRedirect(string returnUrl)
        {
            // ❌ VULNERABLE CODE - DO NOT USE IN PRODUCTION
            // This redirects to ANY URL without validation
            if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["WarningMessage"] = $"VULNERABLE CODE executed! Redirecting to: {returnUrl}";
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // Demonstration of SECURE redirect
        public IActionResult SecureRedirect(string returnUrl)
        {
            // ✅ SECURE CODE - Uses Url.IsLocalUrl() validation
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                TempData["SuccessMessage"] = $"SECURE CODE executed! Redirecting to local URL: {returnUrl}";
                return Redirect(returnUrl);
            }
            else if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["ErrorMessage"] = $"BLOCKED! External URL detected and rejected: {returnUrl}";
            }
            return RedirectToAction("Index", "Home");
        }

        // Test endpoint to show IsLocalUrl results
        public IActionResult TestUrl(string url)
        {
            var isLocal = Url.IsLocalUrl(url);

            ViewBag.TestUrl = url;
            ViewBag.IsLocal = isLocal;
            ViewBag.Result = isLocal ? "✅ ALLOWED (Local URL)" : "❌ BLOCKED (External URL)";

            return View();
        }
    }
}