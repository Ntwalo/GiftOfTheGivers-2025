using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly InMemoryStore _store;
        public EventsController(InMemoryStore store) => _store = store;

        public IActionResult Index() => View(_store.Events);

        public IActionResult Details(int id)
        {
            var ev = _store.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [Authorize(Roles = "Admin,Volunteer")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin,Volunteer")]
        public IActionResult Create(EventModel model)
        {
            model.Id = _store.GetNextEventId();
            _store.Events.Add(model);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var ev = _store.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return NotFound();
            if (!User.IsInRole("Admin") && !User.IsInRole("Volunteer")) return Forbid();
            return View(ev);
        }

        [HttpPost]
        public IActionResult Edit(EventModel model)
        {
            var ev = _store.Events.FirstOrDefault(e => e.Id == model.Id);
            if (ev == null) return NotFound();
            ev.Name = model.Name;
            ev.Description = model.Description;
            ev.Date = model.Date;
            ev.Location = model.Location;
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var ev = _store.Events.FirstOrDefault(e => e.Id == id);
            if (ev != null) _store.Events.Remove(ev);
            return RedirectToAction("Index");
        }
    }
}
