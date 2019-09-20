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
    public class ConducteursController : ControllerBase
    {
        private readonly ICheckContext _context;

        public ConducteursController(ICheckContext context)
        {
            _context = context;
        }

        // GET: api/Conducteurs
        [HttpGet]
        public IEnumerable<Conducteur> GetConducteur()
        {
            return _context.Conducteur;
        }

        // GET: api/Conducteurs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConducteur([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var conducteur = await _context.Conducteur.FindAsync(id);

            if (conducteur == null)
            {
                return NotFound();
            }

            return Ok(conducteur);
        }

        // PUT: api/Conducteurs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConducteur([FromRoute] int id, [FromBody] Conducteur conducteur)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != conducteur.Id)
            {
                return BadRequest();
            }

            _context.Entry(conducteur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConducteurExists(id))
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

        // POST: api/Conducteurs
        [HttpPost]
        public async Task<IActionResult> PostConducteur([FromBody] Conducteur conducteur)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Conducteur.Add(conducteur);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConducteur", new { id = conducteur.Id }, conducteur);
        }


        [HttpPost("postev")]
        public async Task<IActionResult> PostConducteur([FromBody] VmConducteur vmConducteur)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idSociete = SocieteExists(vmConducteur.Societe);

            Conducteur conducteur = new Conducteur()
            {
                NomComplet = vmConducteur.NomComplet,
                Cin = vmConducteur.Cin,
                Cnss = vmConducteur.Cnss,
                DateValiditeAssurance = vmConducteur.DateValiditeAssurance,
                Assurance = vmConducteur.Assurance,
                Patente = vmConducteur.Patente,
                IdSociete = idSociete
            };

            _context.Conducteur.Add(conducteur);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConducteur", new { id = conducteur.Id }, conducteur);
        }

        // DELETE: api/Conducteurs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConducteur([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var conducteur = await _context.Conducteur.FindAsync(id);
            if (conducteur == null)
            {
                return NotFound();
            }

            _context.Conducteur.Remove(conducteur);
            await _context.SaveChangesAsync();

            return Ok(conducteur);
        }

        private bool ConducteurExists(int id)
        {
            return _context.Conducteur.Any(e => e.Id == id);
        }

        private int SocieteExists(string libelle)
        {
            if(_context.Societe.Any(e => e.Libelle.Equals(libelle)))
            {
                Societe societe = new Societe()
                {
                    Libelle = libelle
                };

                _context.Societe.Add(societe);
                _context.SaveChanges();
                return societe.IdSociete;
            }

            return _context.Societe.Where(s => s.Libelle.Equals(libelle)).FirstOrDefault().IdSociete;
        }
    }
}