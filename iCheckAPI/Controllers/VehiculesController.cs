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
    public class VehiculesController : ControllerBase
    {
        private readonly ICheckContext _context;

        public VehiculesController(ICheckContext context)
        {
            _context = context;
        }

        // GET: api/Vehicules
        [HttpGet]
        public IEnumerable<Vehicule> GetVehicule()
        {
            return _context.Vehicule.Include(x => x.IdEnginNavigation);
        }

        // GET: api/Vehicules/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicule = await _context.Vehicule.FindAsync(id);

            if (vehicule == null)
            {
                return NotFound();
            }

            return Ok(vehicule);
        }

        [HttpGet("{engin}")]
        public IEnumerable<Vehicule> GetVehiculeByEngin([FromRoute] string engin)
        {
            var vehicules = _context.Vehicule.Where(x => x.IdEnginNavigation.NomEngin.ToLower() == engin.ToLower());

            return vehicules;
        }


        // PUT: api/Vehicules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicule([FromRoute] int id, [FromBody] Vehicule vehicule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vehicule.Id)
            {
                return BadRequest();
            }

            _context.Entry(vehicule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculeExists(id))
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

        // POST: api/Vehicules
        [HttpPost]
        public async Task<IActionResult> PostVehicule([FromBody] Vehicule vehicule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Vehicule.Add(vehicule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVehicule", new { id = vehicule.Id }, vehicule);
        }

        // DELETE: api/Vehicules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicule = await _context.Vehicule.FindAsync(id);
            if (vehicule == null)
            {
                return NotFound();
            }

            _context.Vehicule.Remove(vehicule);
            await _context.SaveChangesAsync();

            return Ok(vehicule);
        }

        private bool VehiculeExists(int id)
        {
            return _context.Vehicule.Any(e => e.Id == id);
        }
    }
}