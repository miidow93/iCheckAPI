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
    public class SocietesController : ControllerBase
    {
        private readonly icheckContext _context;

        public SocietesController(icheckContext context)
        {
            _context = context;
        }

        // GET: api/Societes
        [HttpGet]
        public IEnumerable<Societe> GetSociete()
        {
            return _context.Societe;
        }

        // GET: api/Societes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSociete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var societe = await _context.Societe.FindAsync(id);

            if (societe == null)
            {
                return NotFound();
            }

            return Ok(societe);
        }

        // PUT: api/Societes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSociete([FromRoute] int id, [FromBody] Societe societe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != societe.IdSociete)
            {
                return BadRequest();
            }

            _context.Entry(societe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SocieteExists(id))
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

        // POST: api/Societes
        [HttpPost]
        public async Task<IActionResult> PostSociete([FromBody] Societe societe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Societe.Add(societe);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSociete", new { id = societe.IdSociete }, societe);
        }

        // DELETE: api/Societes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSociete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var societe = await _context.Societe.FindAsync(id);
            if (societe == null)
            {
                return NotFound();
            }

            _context.Societe.Remove(societe);
            await _context.SaveChangesAsync();

            return Ok(societe);
        }

        private bool SocieteExists(int id)
        {
            return _context.Societe.Any(e => e.IdSociete == id);
        }
    }
}