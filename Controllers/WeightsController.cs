using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mycourier.Models;

namespace mycourier.Controllers
{
    public class WeightsController : Controller
    {
        private readonly MycourierContext _context;

        public WeightsController(MycourierContext context)
        {
            _context = context;
        }

        // GET: Weights
        public async Task<IActionResult> Index()
        {
            return View(await _context.Weights.ToListAsync());
        }

        // GET: Weights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weight = await _context.Weights
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weight == null)
            {
                return NotFound();
            }

            return View(weight);
        }

        // GET: Weights/Create
        // GET: Weights/Create
        public IActionResult Create()
        {
            // Fetch all existing weights
            var weights = _context.Weights.ToList();

            // Pass the weights list to the view
            ViewBag.WeightsList = weights;

            return View();
        }


        // POST: Weights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Fees")] Weight weight)
        {
            if (ModelState.IsValid)
            {
                _context.Add(weight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(weight);
        }

        // GET: Weights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weight = await _context.Weights.FindAsync(id);
            if (weight == null)
            {
                return NotFound();
            }
            return View(weight);
        }

        // POST: Weights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Fees")] Weight weight)
        {
            if (id != weight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeightExists(weight.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Create));
            }
            return View(weight);
        }

        // POST: Weights/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var weight = await _context.Weights.FindAsync(id);
            if (weight != null)
            {
                _context.Weights.Remove(weight);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Create));
        }

        

      
        private bool WeightExists(int id)
        {
            return _context.Weights.Any(e => e.Id == id);
        }
    }
}
