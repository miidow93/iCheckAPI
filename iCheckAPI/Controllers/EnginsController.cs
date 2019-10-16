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
    public class EnginsController : ControllerBase
    {
        private readonly ICheckContext _context;

        public EnginsController(ICheckContext context)
        {
            _context = context;
        }

        // GET: api/Engins
        [HttpGet]
        public async Task<IEnumerable<Engins>> GetEngins()
        {
            return await _context.Engins.ToListAsync();
        }

        [HttpGet("test")]
        public async Task<string> test()
        {
            return "Ok";
        }

        // GET: api/Engins/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEngins([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var engins = await _context.Engins.FindAsync(id);

            if (engins == null)
            {
                return NotFound();
            }

            return Ok(engins);
        }

        [HttpGet("name/{nomEngin}")]
        public async Task<IActionResult> GetEnginsByName([FromRoute] string nomEngin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var engins = await _context.Engins.FirstOrDefaultAsync(x => x.NomEngin == nomEngin);

            if (engins == null)
            {
                return NotFound();
            }

            return Ok(engins);
        }

        // PUT: api/Engins/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEngins([FromRoute] int id, [FromBody] Engins engins)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != engins.Id)
            {
                return BadRequest();
            }

            _context.Entry(engins).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnginsExists(id))
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

        // POST: api/Engins
        [HttpPost]
        public async Task<IActionResult> PostEngins([FromBody] Engins engins)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Engins.Add(engins);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEngins", new { id = engins.Id }, engins);
        }

        // DELETE: api/Engins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEngins([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var engins = await _context.Engins.FindAsync(id);
            if (engins == null)
            {
                return NotFound();
            }

            _context.Engins.Remove(engins);
            await _context.SaveChangesAsync();

            return Ok(engins);
        }

        private bool EnginsExists(int id)
        {
            return _context.Engins.Any(e => e.Id == id);
        }
    }
}