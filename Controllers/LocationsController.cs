using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using mycourier.Models;
using System.Linq;

namespace mycourier.Controllers
{
    public class LocationsController : Controller
    {
        private readonly MycourierContext _context;

        public LocationsController(MycourierContext context)
        {
            _context = context;
        }

        // GET: /Locations
        // We reuse the Create view as the main page (create + list)
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.LocationsList = _context.Locations.ToList();
            return View("Create");
        }

        // GET: /Locations/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.LocationsList = _context.Locations.ToList();
            return View();
        }

        // POST: /Locations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Fees")] Location location)
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Locations.Add(location);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Location added successfully!";
                return RedirectToAction(nameof(Create));
            }

            // Repopulate list if validation fails
            ViewBag.LocationsList = _context.Locations.ToList();
            return View(location);
        }

        // GET: /Locations/Edit/5
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var location = _context.Locations.Find(id);
            if (location == null) return NotFound();

            ViewBag.LocationsList = _context.Locations.ToList();
            return View(location);
        }

        // POST: /Locations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Fees")] Location location)
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id != location.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Locations.Update(location);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Location updated successfully!";
                return RedirectToAction(nameof(Create));
            }

            ViewBag.LocationsList = _context.Locations.ToList();
            return View(location);
        }

        // GET: /Locations/Delete/5
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var location = _context.Locations.Find(id);
            if (location == null) return NotFound();

            ViewBag.LocationsList = _context.Locations.ToList();
            return View(location);
        }

        // POST: /Locations/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var location = _context.Locations.Find(id);
            if (location != null)
            {
                _context.Locations.Remove(location);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Location deleted successfully!";
            }

            return RedirectToAction(nameof(Create));
        }
    }
}
