using GiftOfTheGivers.Data;
using GiftOfTheGivers.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly InMemoryStore _store;
        public NotificationsController(InMemoryStore store) => _store = store;

        public IActionResult Index()
        {
            var uid = AuthHelpers.GetUserId(User);
            var list = _store.Notifications.Where(n => n.UserId == uid).OrderByDescending(n => n.CreatedAt).ToList();
            return View(list);
        }

        [HttpPost]
        public IActionResult MarkRead(int id)
        {
            var n = _store.Notifications.FirstOrDefault(x => x.Id == id);
            if (n != null)
            {
                var uid = AuthHelpers.GetUserId(User);
                if (n.UserId != uid && !User.IsInRole("Admin")) return Forbid();
                n.IsRead = true;
            }
            return RedirectToAction("Index");
        }
    }
}
