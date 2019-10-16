﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCheckAPI.Models;
using iCheckAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public CheckListController(CheckListRepo checkListRepo, IConducteurRepo conducteurRepo, IVehiculeRepo vehiculeRepo)
        {
            _checkListRepo = checkListRepo;
            _conducteurRepo = conducteurRepo;
            _vehiculeRepo = vehiculeRepo;
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

            if(!conducteur)
            {
                Conducteur cond = new Conducteur()
                {
                    Cin = checkList.Conducteur["cin"],
                    NomComplet = checkList.Conducteur["nomComplet"]
                };

                await _conducteurRepo.Create(cond);
            }

            if(!vehicule)
            {
                Vehicule vehi = new Vehicule()
                {
                    Matricule = checkList.Vehicule["matricule"],
                    IdEngin = _vehiculeRepo.GetEnginByName(checkList.Vehicule["engin"])
                };

                await _vehiculeRepo.Create(vehi);
            }

            await _checkListRepo.Create(checkList);

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

            if (!conducteur)
            {
                Conducteur cond = new Conducteur()
                {
                    Cin = checkList.Conducteur["cin"],
                    NomComplet = checkList.Conducteur["nomComplet"]
                };

                await _conducteurRepo.Create(cond);
            }

            if (!vehicule)
            {
                Vehicule vehi = new Vehicule()
                {
                    Matricule = checkList.Vehicule["matricule"],
                    IdEngin = _vehiculeRepo.GetEnginByName(checkList.Vehicule["engin"])
                };

                await _vehiculeRepo.Create(vehi);
            }

            await _checkListRepo.Update(checkList);

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
