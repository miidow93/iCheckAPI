using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ClosedXML.Excel;
using FastMember;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iCheckAPI.Models;

namespace iCheckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly ICheckContext _context;
        public static IHostingEnvironment _environment;


        public ExcelController(ICheckContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        [HttpGet("download/{startDate}/{endDate}")]
        public async Task<IActionResult> DownloadFile([FromRoute] string startDate, [FromRoute] string endDate)
        {
            var fp = "";
            var mm = new MemoryStream();
            System.Diagnostics.Debug.WriteLine(startDate + " " + endDate);
            System.Diagnostics.Debug.WriteLine("DateTime: " + DateTime.Parse(startDate) + " " + DateTime.Parse(endDate));
            var blockages = await _context.Blockage.Where(x => x.DateBlockage >= DateTime.Parse(startDate) && x.DateBlockage <= DateTime.Parse(endDate)).ToListAsync();
            if (blockages.Count > 0)
            {
                DataTable table = new DataTable();
                string[] columns = { "Id", "IdVehicule", "DateBlockage", "Motif", "DateDeblockage" };

                using (var reader = ObjectReader.Create(blockages, columns))
                {
                    table.Load(reader);
                    System.Diagnostics.Debug.WriteLine(reader);
                }
                System.Diagnostics.Debug.WriteLine(table.Rows.Count);
                XLWorkbook wb = new XLWorkbook();
                string workSheetFormat = $"blockage_{DateTime.Now: ddMMyyyy_hhmmssfff}";
                // DateTime.Now.ToString("yyyyMMddHHmmssfff") Guid.NewGuid().ToString();
                wb.Worksheets.Add(table, workSheetFormat);
                string folderName = "Excel";
                string webRootPath = _environment.WebRootPath;
                string pathToSave = Path.Combine(webRootPath, folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                var fileName = workSheetFormat + ".xlsx";
                var fullPath = Path.Combine(pathToSave, fileName);
                wb.SaveAs(fullPath);
                var memory = new MemoryStream();

                using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                mm = memory;
                fp = fullPath;
            }

            return File(mm, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(fp));

            // return Ok(new { test = DateTime.Parse(startDate) });
            //  return File(stream, "application/octet-stream");
            // return File(fullPath, "application/octet-stream");
            // return File(fullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        }

    }
}