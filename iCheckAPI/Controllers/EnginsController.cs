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
    public class EnginsController : ControllerBase
    {
        private readonly ICheckContext _context;
        public static IHostingEnvironment _environment;

        public EnginsController(ICheckContext context,IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
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

            if (engins.ImageEngin.Contains("Images"))
            {
                System.Diagnostics.Debug.WriteLine(engins.ImageEngin);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Base64");
                var imagePath = ConvertImage(engins.ImageEngin);
                engins.ImageEngin = imagePath;
            }


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
            /* if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             _context.Engins.Add(engins);
             await _context.SaveChangesAsync();

             return CreatedAtAction("GetEngins", new { id = engins.Id }, engins);*/
            var imagePath = ConvertImage(engins.ImageEngin);
            engins.ImageEngin = imagePath;
            _context.Engins.Add(engins);
            await _context.SaveChangesAsync();


            return CreatedAtAction("GetEngin", new { id = engins.Id }, engins);
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
        public string ConvertImage(string image)
        {
            // var base64Data = Regex.Match(image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            // var type = Regex.Match(image, @"data:image/(?<type>.+?),").Groups["data"].Value;
            var match = Regex.Match(image, @"data:image/(?<type>.+?);base64,(?<data>.+)");
            var base64Data = match.Groups["data"].Value;
            var contentType = match.Groups["type"].Value;
            System.Diagnostics.Debug.WriteLine(contentType);
            var bytes = Convert.FromBase64String(base64Data);
            string folderName = "Images";
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