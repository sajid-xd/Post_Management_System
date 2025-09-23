using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using mycourier.Models;
using System.Linq;
using System.Collections.Generic;

namespace mycourier.Controllers
{
    public class CreateUsersController : Controller
    {
        private readonly MycourierContext _context;

        public CreateUsersController(MycourierContext context)
        {
            _context = context;
        }

        // ===== READ: List all users =====
        public IActionResult Index()
        {
            var userTypeSession = HttpContext.Session.GetString("user_type");
            if (userTypeSession != "admin")
                return RedirectToAction("Index", "Home");

            var users = _context.Users.ToList();
            return View(users);
        }

        // ===== READ: Details for one user =====
        public IActionResult Details(int id)
        {
            var userTypeSession = HttpContext.Session.GetString("user_type");
            if (userTypeSession != "admin")
                return RedirectToAction("Index", "Home");

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // ===== CREATE =====
        public IActionResult Create()
        {
            var sessionUserType = HttpContext.Session.GetString("user_type");
            bool isAgent = sessionUserType == "agent";

            // Filter users based on role
            if (isAgent)
            {
                ViewBag.Users = _context.Users
                    .Where(u => u.UserType == "user")
                    .ToList();
            }
            else
            {
                ViewBag.Users = _context.Users.ToList(); // Admin sees all
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FullName,Username,Password,PhoneNumber,UserType")] User user)
        {
            var userTypeSession = HttpContext.Session.GetString("user_type");

            if (userTypeSession != "admin" && userTypeSession != "agent")
                return RedirectToAction("Index", "Home");

            // Force agents to create only "user"
            if (userTypeSession == "agent")
            {
                user.UserType = "user";
            }

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction(nameof(Create));
            }

            // Pass user list back if ModelState invalid
            ViewBag.Users = _context.Users
                .Where(u => u.UserType == "user")
                .ToList();

            return View(user);
        }

        // ===== UPDATE =====
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
                return RedirectToAction("Index", "Home");

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,FullName,Username,Password,PhoneNumber,UserType")] User user)
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
                return RedirectToAction("Index", "Home");

            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction(nameof(Create));
            }
            return View(user);
        }

        // ===== DELETE =====
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
                return RedirectToAction("Index", "Home");

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("user_type") != "admin")
                return RedirectToAction("Index", "Home");

            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            return RedirectToAction(nameof(Create));
        }
    }
}
