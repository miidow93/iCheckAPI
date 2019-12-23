using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iCheckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        public static IHostingEnvironment _environment;

        public UploadController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        // GET: api/Upload
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Upload/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Upload
        [HttpPost]
        public string Post([FromBody] Image image)
        {
            var imagePath = ConvertImage(image.URL);
            return imagePath;
        }

        // PUT: api/Upload/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
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

    public class Image
    {
        public string URL { get; set; }
    }
}
