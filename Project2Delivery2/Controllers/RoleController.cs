using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.Models;

namespace Project2Delivery2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: Role/Index - List all roles
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = $"Role '{roleName}' created successfully!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Role already exists!");
                }
            }
            return View();
        }

        // GET: Role/Edit/RoleName
        public async Task<IActionResult> Edit(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound();
            }

            ViewBag.RoleName = role.Name;
            return View();
        }

        // POST: Role/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string oldRoleName, string newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(oldRoleName);
            if (role != null && !string.IsNullOrEmpty(newRoleName))
            {
                role.Name = newRoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Role renamed from '{oldRoleName}' to '{newRoleName}'!";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        // GET: Role/ManageUsers?roleName=Admin
        public async Task<IActionResult> ManageUsers(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound();
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            var allUsers = _userManager.Users.ToList();

            var model = new RoleUsersViewModel
            {
                RoleName = roleName,
                UsersInRole = usersInRole.ToList(),
                AvailableUsers = allUsers.Except(usersInRole).ToList()
            };

            return View(model);
        }

        // POST: Role/AddUserToRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"User '{user.UserName}' added to role '{roleName}'!";
                }
            }
            return RedirectToAction("ManageUsers", new { roleName });
        }

        // POST: Role/RemoveUserFromRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"User '{user.UserName}' removed from role '{roleName}'!";
                }
            }
            return RedirectToAction("ManageUsers", new { roleName });
        }
    }
}