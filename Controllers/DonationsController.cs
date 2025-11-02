using GiftOfTheGivers.Data;
using GiftOfTheGivers.Helpers;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    [Authorize]
    public class DonationsController : Controller
    {
        private readonly InMemoryStore _store;
        public DonationsController(InMemoryStore store) => _store = store;

        public IActionResult Index()
        {
            return View(_store.Donations);
        }

        public IActionResult Details(int id)
        {
            var donation = _store.Donations.FirstOrDefault(d => d.Id == id);
            if (donation == null) return NotFound();
            return View(donation);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Donation model)
        {
            model.Id = _store.GetNextDonationId();
            model.OwnerId = AuthHelpers.GetUserId(User);
            _store.Donations.Add(model);

            // Notify admins
            foreach (var admin in _store.Users.Where(u => u.Role == "Admin"))
            {
                _store.Notifications.Add(new Notification
                {
                    Id = _store.GetNextNotificationId(),
                    UserId = admin.Id,
                    Message = $"New donation created: {model.Title}",
                    IsRead = false
                });
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var donation = _store.Donations.FirstOrDefault(d => d.Id == id);
            if (donation == null) return NotFound();

            // Only owner or admin can edit
            var currentId = AuthHelpers.GetUserId(User);
            if (donation.OwnerId != currentId && !User.IsInRole("Admin"))
                return Forbid();

            return View(donation);
        }

        [HttpPost]
        public IActionResult Edit(Donation model)
        {
            var donation = _store.Donations.FirstOrDefault(d => d.Id == model.Id);
            if (donation == null) return NotFound();

            var currentId = AuthHelpers.GetUserId(User);
            if (donation.OwnerId != currentId && !User.IsInRole("Admin"))
                return Forbid();

            donation.Title = model.Title;
            donation.Description = model.Description;
            donation.Category = model.Category;
            donation.Quantity = model.Quantity;
            donation.Status = model.Status;

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var item = _store.Donations.FirstOrDefault(d => d.Id == id);
            if (item != null) _store.Donations.Remove(item);
            return RedirectToAction("Index");
        }
    }
}
