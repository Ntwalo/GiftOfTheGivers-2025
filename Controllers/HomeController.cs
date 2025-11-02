using Microsoft.AspNetCore.Mvc;
using GiftOfTheGivers.Data;

namespace GiftOfTheGivers.Controllers
{
    public class HomeController : Controller
    {
        private readonly InMemoryStore _store;
        public HomeController(InMemoryStore store) => _store = store;

        public IActionResult Index()
        {
            ViewBag.RecentDonations = _store.Donations.Take(5).ToList();
            ViewBag.UpcomingEvents = _store.Events.OrderBy(e => e.Date).Take(5).ToList();
            return View();
        }
    }
}
