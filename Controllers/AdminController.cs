using GiftOfTheGivers.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly InMemoryStore _store;
        public AdminController(InMemoryStore store) => _store = store;

        public IActionResult Index()
        {
            var model = new
            {
                Users = _store.Users,
                Donations = _store.Donations,
                Events = _store.Events,
                Requests = _store.Requests,
                Notifications = _store.Notifications
            };
            return View(model);
        }
    }
}
