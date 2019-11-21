using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class BlockagesController : ControllerBase
    {
        private readonly ICheckContext _context;
        public static IHostingEnvironment _environment;

        public BlockagesController(ICheckContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
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

            if (blockage.ImageUrl.Contains("Upload"))
            {
                System.Diagnostics.Debug.WriteLine(blockage.ImageUrl);
            }
            else
            {
                if(blockage.ImageUrl != "")
                {
                    System.Diagnostics.Debug.WriteLine("Base64");
                    var imagePath = ConvertImage(blockage.ImageUrl);
                    blockage.ImageUrl = imagePath;
                }
                
            }


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
            /*if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Blockage.Add(blockage);*/
            var imagePath = ConvertImage(blockage.ImageUrl);
            blockage.ImageUrl = imagePath;
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