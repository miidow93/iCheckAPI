using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iCheckAPI.Models;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace iCheckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculesController : ControllerBase
    {
        private readonly ICheckContext _context;
        public static IHostingEnvironment _environment;

        public VehiculesController(ICheckContext context,IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Vehicules
        [HttpGet]
        public IEnumerable<object> GetVehicule()
        {
            return _context.Vehicule.Where(w => w.IdEngin != null).Select(s => new { 
                s.Id,
                s.IdEngin,
                nomEngin = s.IdEnginNavigation.NomEngin,
                s.Matricule,
                s.ImageUrl
            });
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

            if(!string.IsNullOrEmpty(vehicule.ImageUrl))
            {
                var imagePath = ConvertImage(vehicule.ImageUrl);
                vehicule.ImageUrl = imagePath;
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

        public string ConvertImage(string image)
        {
            // var base64Data = Regex.Match(image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            // var type = Regex.Match(image, @"data:image/(?<type>.+?),").Groups["data"].Value;
            var match = Regex.Match(image, @"data:image/(?<type>.+?);base64,(?<data>.+)");
            var base64Data = match.Groups["data"].Value;
            var contentType = match.Groups["type"].Value;
            System.Diagnostics.Debug.WriteLine(contentType);
            var bytes = Convert.FromBase64String(base64Data);
            string folderName = "Upload";
            string webRootPath = _environment.WebRootPath;
            string pathToSave = Path.Combine(webRootPath, folderName);

            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
            var fileName = Guid.NewGuid().ToString() + "." + contentType;
            var fullPath = Path.Combine(pathToSave, fileName);
            var dbPath = Path.Combine(folderName, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
            return dbPath;
        }
    }
}
