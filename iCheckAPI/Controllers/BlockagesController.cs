using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iCheckAPI.Models;

namespace iCheckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockagesController : ControllerBase
    {
        private readonly ICheckContext _context;

        public BlockagesController(ICheckContext context)
        {
            _context = context;
        }

        // GET: api/Blockages
        [HttpGet]
        public IEnumerable<Blockage> GetBlockage()
        {
            return _context.Blockage.Include(x => x.IdVehiculeNavigation.Blockage)
                                    .Include(c => c.IdVehiculeNavigation.CheckListRef);
        }

        // GET: api/Blockages/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlockage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blockage = await _context.Blockage.FindAsync(id);

            if (blockage == null)
            {
                return NotFound();
            }

            return Ok(blockage);
        }

        // PUT: api/Blockages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlockage([FromRoute] int id, [FromBody] Blockage blockage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blockage.Id)
            {
                return BadRequest();
            }

            _context.Entry(blockage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlockageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Blockages
        [HttpPost]
        public async Task<IActionResult> PostBlockage([FromBody] Blockage blockage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Blockage.Add(blockage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlockage", new { id = blockage.Id }, blockage);
        }

        // DELETE: api/Blockages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlockage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blockage = await _context.Blockage.FindAsync(id);
            if (blockage == null)
            {
                return NotFound();
            }

            _context.Blockage.Remove(blockage);
            await _context.SaveChangesAsync();

            return Ok(blockage);
        }

        private bool BlockageExists(int id)
        {
            return _context.Blockage.Any(e => e.Id == id);
        }
    }
}