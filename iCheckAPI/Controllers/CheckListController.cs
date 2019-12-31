using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iCheckAPI.Models;
using iCheckAPI.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;

namespace iCheckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckListController : ControllerBase
    {

        private readonly CheckListRepo _checkListRepo;
        private readonly ICheckContext _context;
        private readonly IConducteurRepo _conducteurRepo;
        private readonly IVehiculeRepo _vehiculeRepo;
        private readonly ISiteRepo _siteRepo;
        public static IHostingEnvironment _environment;


        public CheckListController(CheckListRepo checkListRepo,
            ICheckContext context,
            IConducteurRepo conducteurRepo,
            IVehiculeRepo vehiculeRepo,
            ISiteRepo siteRepo,
            IHostingEnvironment environment)
        {
            _checkListRepo = checkListRepo;
            _conducteurRepo = conducteurRepo;
            _vehiculeRepo = vehiculeRepo;
            _context = context;
            _siteRepo = siteRepo;
            _environment = environment;
        }

        // GET: api/CheckList
        [HttpGet]
        public async Task<IActionResult> GetCheckList()
        {
            return new ObjectResult(await _checkListRepo.GetAllCheckList());
        }

        // GET: api/CheckList/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCheckListByID(string id)
        {
            var checkList = await _checkListRepo.GetCheckListByID(id);
            if (checkList == null)
            {
                return NotFound();
            }

            return Ok(checkList);
        }

        [HttpGet("bydate/{date}")]
        public async Task<IActionResult> GetCheckListByDate(string date)
        {
            // var datetime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null);

            var checkList = await _checkListRepo.GetCheckListByDate(DateTime.Parse(date));
            if (checkList == null)
            {
                return NotFound();
            }



            return Ok(checkList);
        }

        [HttpGet("qrcode/{matricule}")]
        public async Task<IActionResult> GetCheckListByMatricule(string matricule)
        {
            // var datetime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null);

            var checkList = await _checkListRepo.GetLastCheckListByMatricule(matricule);
            if (checkList == null)
            {
                return NotFound();
            }



            return Ok(checkList);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetCheckListByType()
        {
            // var datetime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null);
            List<CheckListDTO> list = new List<CheckListDTO>();
            var checkList = await _checkListRepo.GetAllCheckList();
            if (checkList == null)
            {
                return NotFound();
            }

            

            foreach (var item in checkList)
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

                list.Add(new CheckListDTO
                {
                    Id = item.Id,
                    Conducteur = item.Conducteur["nomComplet"],
                    Vehicule = item.Vehicule,
                    Date = item.Date,
                    Etat = item.Etat,
                    Site = item.Site,
                    Motif = item.Motif,
                    Controlleur = item.Controlleur,
                    CheckConducteur = item.CatchAll["checklistConducteur"],
                    CheckEngin = engins
                });
            }

            return Ok(list);
        }



        // POST: api/CheckList
        [HttpPost]
        public async Task<ActionResult<CheckList>> Post([FromBody] CheckList checkList)
        {
            /*CheckList check = new CheckList()
            {
                NombreP = checkList.NombreP,
                Transporteur = checkList.Transporteur,
                Tracteur = checkList.Tracteur,
                Date = checkList.Date,
                CatchAll = BsonDocument.Parse(checkList.CatchAll.ToString())
            };*/
            var blockage = new Blockage();

            var jsonDoc = JsonConvert.SerializeObject(checkList.CatchAll);

            checkList.CatchAll = BsonSerializer.Deserialize<Dictionary<string, object>>(jsonDoc);
            System.Diagnostics.Debug.WriteLine(jsonDoc);
            System.Diagnostics.Debug.WriteLine(checkList.CatchAll);

            var conducteur = _conducteurRepo.GetConducteurByCIN(checkList.Conducteur["cin"]);
            var vehicule = _vehiculeRepo.GetVehiculeByMatricule(checkList.Vehicule["matricule"]);
            var site = _siteRepo.GetSiteByLibelle(checkList.Site);

            var conducteurID = conducteur != null ? conducteur.Id : -1;
            var vehiculeID = vehicule != null ? vehicule.Id : -1;
            var siteID = site != null ? site.Id : -1;
            // var blockageID = -1;
            if (!string.IsNullOrEmpty(checkList.ImageURL))
            {
                var imagePath = ConvertImage(checkList.ImageURL);
                checkList.ImageURL = imagePath;
                /*foreach (var image in checkList.ImagesURL)
                {
                    var imagePath = ConvertImage(image.Value);
                    checkList.ImagesURL[image.Key] = imagePath;
                    var imageKey = image.Key;
                    var index = 0;
                    foreach (var item in image.Value)
                    {
                        var imagePath = ConvertImage(item);
                        checkList.ImagesURL[imageKey][index] = imagePath;
                        index++;
                        System.Diagnostics.Debug.WriteLine("Image Path: " + imagePath);
                        System.Diagnostics.Debug.WriteLine("Image Index: " + checkList.ImagesURL[imageKey][index]);
                    }
                }*/
            }


            if (conducteur == null)
            {
                Conducteur cond = new Conducteur()
                {
                    Cin = checkList.Conducteur["cin"],
                    NomComplet = checkList.Conducteur["nomComplet"]
                };

                await _conducteurRepo.Create(cond);
                conducteurID = cond.Id;
            }

            if (vehicule == null)
            {
                Vehicule vehi = new Vehicule()
                {
                    Matricule = checkList.Vehicule["matricule"],
                    IdEngin = _vehiculeRepo.GetEnginByName(checkList.Vehicule["engin"])
                };

                await _vehiculeRepo.Create(vehi);
                vehiculeID = vehi.Id;
            }

            if (site == null)
            {
                Site st = new Site()
                {
                    Libelle = checkList.Site,
                };

                await _siteRepo.Create(site);
                siteID = st.Id;
            }

            checkList.Vehicule["idVehicule"] = vehiculeID.ToString();
            checkList.Conducteur["idConducteur"] = conducteurID.ToString();

            await _checkListRepo.Create(checkList);
            System.Diagnostics.Debug.WriteLine(checkList.Id.ToString());
            System.Diagnostics.Debug.WriteLine(checkList.Controlleur);
            _context.CheckListRef.Add(new CheckListRef()
            {
                IdCheckListRef = checkList.Id.ToString(),
                Date = checkList.Date.Value.Date,
                Rating = checkList.Rating,
                Etat = checkList.Etat,
                IdConducteur = conducteurID,
                IdVehicule = vehiculeID,
                IdSite = siteID,
                IdControlleur = Convert.ToInt32(checkList.Controlleur["id"])
            });


            if (checkList.Etat)
            {
                blockage.IdVehicule = vehiculeID;
                blockage.DateBlockage = checkList.Date.Value.Date;
                blockage.IdCheckList = checkList.Id;
                blockage.ImageUrl = checkList.ImageURL;
                _context.Blockage.Add(blockage);
            }

            _context.SaveChanges();
            // checkList.Vehicule["idBlockage"] = blockage.IdVehicule != null ? blockage.Id.ToString() : "-1";
            // System.Diagnostics.Debug.WriteLine("BlockageID:" + blockageID);
            return CreatedAtAction("GetCheckList", new { id = checkList.Id.ToString() }, checkList);
        }

        // PUT: api/CheckList/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] CheckList checkList)
        {
            var checkListSearch = _checkListRepo.GetCheckListByID(id);

            if (checkListSearch == null)
            {
                return NotFound();
            }

            var jsonDoc = JsonConvert.SerializeObject(checkList.CatchAll);

            checkList.CatchAll = BsonSerializer.Deserialize<Dictionary<string, object>>(jsonDoc);

            var conducteur = _conducteurRepo.GetConducteurByCIN(checkList.Conducteur["cin"]);
            var vehicule = _vehiculeRepo.GetVehiculeByMatricule(checkList.Vehicule["matricule"]);
            var site = _siteRepo.GetSiteByLibelle(checkList.Site);

            var conducteurID = conducteur.Id;
            var vehiculeID = vehicule.Id;
            var siteID = site.Id;

            if (conducteur == null)
            {
                Conducteur cond = new Conducteur()
                {
                    Cin = checkList.Conducteur["cin"],
                    NomComplet = checkList.Conducteur["nomComplet"]
                };

                await _conducteurRepo.Create(cond);
                conducteurID = cond.Id;
            }

            if (vehicule == null)
            {
                Vehicule vehi = new Vehicule()
                {
                    Matricule = checkList.Vehicule["matricule"],
                    IdEngin = _vehiculeRepo.GetEnginByName(checkList.Vehicule["engin"])
                };

                await _vehiculeRepo.Create(vehi);
                vehiculeID = vehi.Id;
            }

            if (site == null)
            {
                Site st = new Site()
                {
                    Libelle = checkList.Site,
                };

                await _siteRepo.Create(site);
                siteID = st.Id;
            }

            await _checkListRepo.Update(checkList);

            CheckListRef checkListRef = _context.CheckListRef.FirstOrDefault(x => x.IdCheckListRef == checkList.Id.ToString());
            checkListRef.Date = checkList.Date.Value.Date;
            checkListRef.IdConducteur = conducteurID;
            checkListRef.IdVehicule = vehiculeID;
            checkListRef.IdCheckListRef = checkList.Id.ToString();
            checkListRef.IdSite = siteID;
            checkListRef.IdControlleur = Convert.ToInt32(checkList.Controlleur["id"]);

            _context.Entry(checkListRef).State = EntityState.Modified;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var checkList = _checkListRepo.GetCheckListByID(id);

            if (checkList == null)
            {
                return NotFound();
            }

            await _checkListRepo.Delete(id);

            return NoContent();
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

    internal class CheckListDTO
    {
        public string Id { get; set; }

        public string Conducteur { get; set; }
        public Dictionary<string, string> Vehicule { get; set; }
        public DateTime? Date { get; set; }
        public bool Etat { get; set; }
        public string Site { get; set; }
        public string Motif { get; set; }
        public Dictionary<string, string> Controlleur { get; set; }

        public object CheckConducteur { get; set; }

        public List<bool> CheckEngin { get; set; }

    }
}
