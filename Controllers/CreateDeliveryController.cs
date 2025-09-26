using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mycourier.Models;
using mycourier.Models.ViewModels;
using MyCourier.Models;
using System;
using System.Linq;

namespace MyCourier.Controllers
{
    public class CreateDeliveryController : Controller
    {
        private readonly MycourierContext _context;

        public CreateDeliveryController(MycourierContext context)
        {
            _context = context;
        }

        // GET: Show delivery creation form
        public IActionResult Index()
        {
            var agentId = HttpContext.Session.GetInt32("id");
            if (agentId == null) return RedirectToAction("Logout", "Account");

            var agent = _context.Users.Find(agentId);
            if (agent?.UserType != "agent") return RedirectToAction("Logout", "Account");

            // Populate dropdowns
            ViewBag.Users = _context.Users.Where(u => u.UserType == "user").ToList();
            ViewBag.Services = _context.Services.ToList();
            ViewBag.Weights = _context.Weights.ToList();
            ViewBag.Locations = _context.Locations.ToList();

            // Pass agent's deliveries
            ViewBag.Deliveries = _context.Deliveries
                                        .Where(d => d.AgentId == agentId)
                                        .Include(d => d.Sender)
                                        .Include(d => d.Service)
                                        .Include(d => d.Weight)
                                        .Include(d => d.Location)
                                        .ToList();

            return View("CreateDelivery");
        }


        // POST: Create a new delivery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CreateDeliveryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns
                ViewBag.Users = _context.Users.Where(u => u.UserType == "user").ToList();
                ViewBag.Services = _context.Services.ToList();
                ViewBag.Weights = _context.Weights.ToList();
                ViewBag.Locations = _context.Locations.ToList();
                return View("CreateDelivery", model);
            }

            var agentId = HttpContext.Session.GetInt32("id");
            if (agentId == null) return RedirectToAction("Logout", "Account");

            // Generate unique 4-digit tracking ID
            string trackingId;
            do
            {
                trackingId = new Random().Next(1000, 9999).ToString();
            } while (_context.Deliveries.Any(d => d.TrackingId == trackingId));

            var delivery = new Delivery
            {
                FromAddress = model.FromAddress,
                ToAddress = model.ToAddress,
                SenderId = model.SenderId,
                ReceiverName = model.ReceiverName,
                ServiceId = model.ServiceId,
                WeightId = model.WeightId,
                LocationId = model.LocationId,
                AgentId = agentId.Value,
                TrackingId = trackingId,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Deliveries.Add(delivery);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Delivery created successfully! Tracking ID: {trackingId}";
            return RedirectToAction("Index", "CreateDelivery");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var agentId = HttpContext.Session.GetInt32("id");
            if (agentId == null) return RedirectToAction("Logout", "Account");

            // Find the delivery for the current agent
            var delivery = _context.Deliveries
                .FirstOrDefault(d => d.Id == id && d.AgentId == agentId);

            if (delivery == null)
            {
                TempData["ErrorMessage"] = "Delivery not found or you don't have permission.";
                return RedirectToAction("Index");
            }

            _context.Deliveries.Remove(delivery);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Delivery with Tracking ID {delivery.TrackingId} has been deleted.";
            return RedirectToAction("Index");
        }

    }
}
