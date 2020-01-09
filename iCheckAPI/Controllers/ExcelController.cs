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
using iCheckAPI.Repositories;
using System.Collections;

namespace iCheckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly CheckListRepo _checkListRepo;
        private readonly ICheckContext _context;
        public static IHostingEnvironment _environment;


        public ExcelController(ICheckContext context, CheckListRepo checkListRepo, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _checkListRepo = checkListRepo;
        }


        [HttpGet("download/{startDate}/{endDate}")]
        public async Task<IActionResult> DownloadFile([FromRoute] string startDate, [FromRoute] string endDate)
        {
            var fp = "";
            var mm = new MemoryStream();
            System.Diagnostics.Debug.WriteLine(startDate + " " + endDate);
            System.Diagnostics.Debug.WriteLine("DateTime: " + DateTime.Parse(startDate) + " " + DateTime.Parse(endDate));


            List<CheckListDTO> list = new List<CheckListDTO>();
            List<SyntheseDTO> syntheses = new List<SyntheseDTO>();
            var checkList = await _checkListRepo.GetAllCheckList();
            var filtered = checkList.Where(w => w.Date.Value.Date >= DateTime.Parse(startDate).Date && w.Date.Value.Date <= DateTime.Parse(endDate).Date).ToList();
            System.Diagnostics.Debug.WriteLine("Filtered Count: " + filtered.Count);


            if (filtered.Count > 0)
            {

                foreach (var item in filtered)
                {
                    List<bool> engins = new List<bool>();
                    System.Diagnostics.Debug.WriteLine("Get Type: " + item.CatchAll["checklistEngin"].GetType());
                    if (item.CatchAll["checklistEngin"].GetType().IsGenericType)
                    {
                        System.Diagnostics.Debug.WriteLine("Is Array");
                        IEnumerable enumerable = item.CatchAll["checklistEngin"] as IEnumerable;
                        if (enumerable != null)
                        {
                            System.Diagnostics.Debug.WriteLine("Is Not Null");

                            var index = 0;
                            foreach (bool n in enumerable)
                            {
                                switch (index)
                                {
                                    case 0: engins.Add(n); break;
                                    case 2: engins.Add(n); break;
                                    case 4: engins.Add(n); break;
                                    case 6: engins.Add(n); break;
                                    case 10: engins.Add(n); break;
                                }
                                index++;
                            }
                        }
                    }

                    List<bool> conducteurs = new List<bool>();
                    if (item.CatchAll["checklistConducteur"].GetType().IsGenericType)
                    {
                        System.Diagnostics.Debug.WriteLine("Is Array");
                        IEnumerable enumerable = item.CatchAll["checklistConducteur"] as IEnumerable;
                        if (enumerable != null)
                        {
                            System.Diagnostics.Debug.WriteLine("Is Not Null");

                            foreach (bool n in enumerable)
                            {
                                conducteurs.Add(n); 
                            }
                        }
                    }


                    list.Add(new CheckListDTO
                    {
                        Conducteur = item.Conducteur["nomComplet"],
                        Vehicule = item.Vehicule,
                        Date = item.Date,
                        Etat = item.Etat,
                        Site = item.Site,
                        Motif = item.Motif,
                        ImageURL = item.ImageURL,
                        Controlleur = item.Controlleur,
                        CheckConducteur = item.CatchAll["checklistConducteur"],
                        CheckEngin = engins
                    });

                    syntheses.Add(new SyntheseDTO
                    {
                        Conducteur = item.Conducteur["nomComplet"],
                        Site = item.Site,
                        Engin = item.Vehicule["engin"],
                        Matricule = item.Vehicule["matricule"],
                        Controlleur = item.Controlleur != null ? item.Controlleur["nomComplet"] : "Anonyme",
                        Motif = item.Motif,
                        DateControle = item.Date,
                        CasqueSec = conducteurs[0] == true ? "conforme" : "non conforme",
                        LunetteProtection = conducteurs[1] == true ? "conforme" : "non conforme",
                        GantsProtection = conducteurs[2] == true ? "conforme" : "non conforme",
                        Gilet = conducteurs[3] == true ? "conforme" : "non conforme",
                        ChaussuresSec = conducteurs[4] == true ? "conforme" : "non conforme",
                        TableauBord = engins[0] == true ? "conforme" : "non conforme",
                        CeintureSec = engins[1] == true ? "conforme" : "non conforme",
                        Retriviseur = engins[2] == true ? "conforme" : "non conforme",
                        GPS = engins[3] == true ? "conforme" : "non conforme", 
                        Roues = engins[4] == true ? "conforme" : "non conforme"
                    });
                }



                DataTable table = new DataTable();
                 string[] columns = { "Conducteur", "Site", "Matricule", "Engin", "Controlleur", 
                                       "Motif", "DateControle", "CasqueSec", "LunetteProtection",
                                       "GantsProtection", "Gilet", "ChaussuresSec", "TableauBord",
                                        "CeintureSec", "Retriviseur", "GPS", "Roues" };

                 using (var reader = ObjectReader.Create(syntheses, columns))
                 {
                     table.Load(reader);
                     System.Diagnostics.Debug.WriteLine(reader);
                 }
                 System.Diagnostics.Debug.WriteLine(table.Rows.Count);
                 XLWorkbook wb = new XLWorkbook();
                 string workSheetFormat = $"synthese_{DateTime.Now: ddMMyyyy_hhmmssfff}";
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
            // return Ok(syntheses);

        }


        [HttpGet("download/checklist/{startDate}/{endDate}")]
        public async Task<IActionResult> DownloadCheckListFile([FromRoute] string startDate, [FromRoute] string endDate)
        {
            var fp = "";
            var mm = new MemoryStream();
            System.Diagnostics.Debug.WriteLine(startDate + " " + endDate);
            System.Diagnostics.Debug.WriteLine("DateTime: " + DateTime.Parse(startDate) + " " + DateTime.Parse(endDate));
            var checklist = await _context.CheckListRef.Select(s => new
            {
                s.Id,
                Matricule = s.IdVehiculeNavigation.Matricule,
                Engin = s.IdVehiculeNavigation.IdEnginNavigation.NomEngin,
                s.Date,
                s.Etat
            }).Where(x => x.Date >= DateTime.Parse(startDate) && x.Date <= DateTime.Parse(endDate)).ToListAsync();

            var mapped = checklist.Select(s =>
            {
                var data = new CheckListRefDTO();
                data.Id = s.Id;
                data.Matricule = s.Matricule;
                data.Engin = s.Engin;
                data.Date = s.Date;
                if ((bool)s.Etat)
                {
                    data.Etat = "Non Autoriser";
                }
                else
                {
                    data.Etat = "Autoriser";
                }
                return data;
            }).ToList();

            if (mapped.Count > 0)
            {
                DataTable table = new DataTable();
                string[] columns = { "Id", "Matricule", "Engin", "Date", "Etat" };

                using (var reader = ObjectReader.Create(mapped, columns))
                {
                    table.Load(reader);
                    System.Diagnostics.Debug.WriteLine(reader);
                }
                System.Diagnostics.Debug.WriteLine(table.Rows.Count);
                XLWorkbook wb = new XLWorkbook();
                string workSheetFormat = $"historique_{DateTime.Now: ddMMyyyy_hhmmssfff}";
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
        }

    }


    internal class CheckListRefDTO
    {
        public int Id { get; set; }
        public string Matricule { get; set; }
        public string Engin { get; set; }
        public string Etat { get; set; }
        public DateTime? Date { get; set; }
    }


    internal class SyntheseDTO
    {
        public string Controlleur { get; set; }
        public string Site { get; set; }
        public string Matricule { get; set; }
        public string Engin { get; set; }
        public string Conducteur { get; set; }
        public string Motif { get; set; }
        public DateTime? DateControle { get; set; }
        public string CasqueSec { get; set; }
        public string LunetteProtection { get; set; }
        public string GantsProtection { get; set; }
        public string Gilet { get; set; }
        public string ChaussuresSec { get; set; }
        public string TableauBord { get; set; }
        public string CeintureSec { get; set; }
        public string Retriviseur { get; set; }
        public string GPS { get; set; }
        public string Roues { get; set; }
    }
}