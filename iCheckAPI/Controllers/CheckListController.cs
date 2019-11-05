using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCheckAPI.Models;
using iCheckAPI.Repositories;
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

        public CheckListController(CheckListRepo checkListRepo, 
            ICheckContext context, 
            IConducteurRepo conducteurRepo, 
            IVehiculeRepo vehiculeRepo,
            ISiteRepo siteRepo)
        {
            _checkListRepo = checkListRepo;
            _conducteurRepo = conducteurRepo;
            _vehiculeRepo = vehiculeRepo;
            _context = context;
            _siteRepo = siteRepo;
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
            if(checkList == null)
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

            if(conducteur == null)
            {
                Conducteur cond = new Conducteur()
                {
                    Cin = checkList.Conducteur["cin"],
                    NomComplet = checkList.Conducteur["nomComplet"]
                };

                await _conducteurRepo.Create(cond);
                conducteurID = cond.Id;
            }

            if(vehicule == null)
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

            await _checkListRepo.Create(checkList);
            System.Diagnostics.Debug.WriteLine(checkList.Id.ToString());

            _context.CheckListRef.Add(new CheckListRef()
            {
                IdCheckListRef = checkList.Id.ToString(),
                Date = checkList.Date.Value.Date,
                Rating = checkList.Rating,
                Etat = checkList.Etat,
                IdConducteur = conducteurID,
                IdVehicule = vehiculeID,
                IdSite = siteID
            }) ;
            _context.SaveChanges();
            return CreatedAtAction("GetCheckList", new { id = checkList.Id.ToString() }, checkList);
        }

        // PUT: api/CheckList/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] CheckList checkList)
        {
            var checkListSearch = _checkListRepo.GetCheckListByID(id);

            if(checkListSearch == null)
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

            _context.Entry(checkListRef).State = EntityState.Modified;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var checkList = _checkListRepo.GetCheckListByID(id);

            if(checkList == null)
            {
                return NotFound();
            }

            await _checkListRepo.Delete(id);

            return NoContent();
        }
    }
}
