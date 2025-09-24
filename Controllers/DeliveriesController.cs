using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mycourier.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mycourier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        private readonly MycourierContext _context;

        public DeliveriesController(MycourierContext context)
        {
            _context = context;
        }

        // GET: api/deliveries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Delivery>>> GetDeliveries()
        {
            return await _context.Deliveries.ToListAsync();
        }

        // GET: api/deliveries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Delivery>> GetDelivery(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null) return NotFound();
            return delivery;
        }

        // POST: api/deliveries
        [HttpPost]
        public async Task<ActionResult<Delivery>> PostDelivery(Delivery delivery)
        {
            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDelivery), new { id = delivery.Id }, delivery);
        }

        // PUT: api/deliveries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDelivery(int id, Delivery delivery)
        {
            if (id != delivery.Id) return BadRequest();

            _context.Entry(delivery).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Deliveries.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        // DELETE: api/deliveries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null) return NotFound();

            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/deliveries/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null) return NotFound();

            delivery.Status = status;
            await _context.SaveChangesAsync();
            return Ok(delivery);
        }
    }
}
