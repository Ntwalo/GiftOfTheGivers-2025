using GiftOfTheGivers.Data;
using GiftOfTheGivers.Helpers;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    public class AccountController : Controller
    {
        private readonly InMemoryStore _store;
        private readonly IPasswordHasher<User> _hasher;
        public AccountController(InMemoryStore store, IPasswordHasher<User> hasher)
        {
            _store = store;
            _hasher = hasher;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            var user = _store.Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View();
            }

            var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (res == PasswordVerificationResult.Success || res == PasswordVerificationResult.SuccessRehashNeeded)
            {
                await AuthHelpers.SignInUser(HttpContext, user);
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid credentials");
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string fullName, string role = "Donor")
        {
            if (_store.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("", "Email already used");
                return View();
            }

            var user = new User
            {
                Email = email,
                FullName = fullName,
                Role = role
            };
            user.PasswordHash = _hasher.HashPassword(user, password);
            _store.Users.Add(user);

            // create welcome notification
            _store.Notifications.Add(new Models.Notification
            {
                Id = _store.GetNextNotificationId(),
                UserId = user.Id,
                Message = "Welcome to Gift of the Givers!",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            await AuthHelpers.SignInUser(HttpContext, user);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
