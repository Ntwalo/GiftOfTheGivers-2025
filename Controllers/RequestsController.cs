using GiftOfTheGivers.Data;
using GiftOfTheGivers.Helpers;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly InMemoryStore _store;
        public RequestsController(InMemoryStore store) => _store = store;

        public IActionResult Index()
        {
            // Admin sees all, others see their own
            if (User.IsInRole("Admin"))
                return View(_store.Requests);
            var uid = AuthHelpers.GetUserId(User);
            return View(_store.Requests.Where(r => r.RequesterId == uid).ToList());
        }

        public IActionResult Details(int id)
        {
            var req = _store.Requests.FirstOrDefault(r => r.Id == id);
            if (req == null) return NotFound();
            if (!User.IsInRole("Admin") && req.RequesterId != AuthHelpers.GetUserId(User)) return Forbid();
            var donation = _store.Donations.FirstOrDefault(d => d.Id == req.DonationId);
            ViewBag.Donation = donation;
            return View(req);
        }

        public IActionResult Create(int donationId)
        {
            var donation = _store.Donations.FirstOrDefault(d => d.Id == donationId);
            if (donation == null) return NotFound();
            ViewBag.Donation = donation;
            return View(new RequestModel { DonationId = donationId });
        }

        [HttpPost]
        public IActionResult Create(RequestModel model)
        {
            model.Id = _store.GetNextRequestId();
            model.RequesterId = AuthHelpers.GetUserId(User);
            model.RequestedAt = DateTime.UtcNow;
            model.Status = "Pending";
            _store.Requests.Add(model);

            // notify admins
            foreach (var admin in _store.Users.Where(u => u.Role == "Admin"))
            {
                _store.Notifications.Add(new Notification
                {
                    Id = _store.GetNextNotificationId(),
                    UserId = admin.Id,
                    Message = $"Request #{model.Id} for donation #{model.DonationId} created",
                    IsRead = false
                });
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            var req = _store.Requests.FirstOrDefault(r => r.Id == id);
            if (req == null) return NotFound();
            req.Status = "Approved";

            // mark donation as claimed
            var donation = _store.Donations.FirstOrDefault(d => d.Id == req.DonationId);
            if (donation != null)
            {
                donation.Status = "Claimed";
            }

            // notify requester
            _store.Notifications.Add(new Notification
            {
                Id = _store.GetNextNotificationId(),
                UserId = req.RequesterId,
                Message = $"Your request #{req.Id} has been approved.",
                IsRead = false
            });

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id)
        {
            var req = _store.Requests.FirstOrDefault(r => r.Id == id);
            if (req == null) return NotFound();
            req.Status = "Rejected";
            _store.Notifications.Add(new Notification
            {
                Id = _store.GetNextNotificationId(),
                UserId = req.RequesterId,
                Message = $"Your request #{req.Id} has been rejected.",
                IsRead = false
            });
            return RedirectToAction("Index");
        }
    }
}
