using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCheckAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using iCheckAPI.Repositories;
using System.Collections;
using MongoDB.Bson;

namespace iCheckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {


        // synthese mensuel des camions par type 
        // synthese mensuel des camions.
        // nombre des camions suspendus par site + nombre total des camions controlés
        private readonly ICheckContext _context;
        private readonly CheckListRepo _checkListRepo;
        public StatsController(ICheckContext context, CheckListRepo checkListRepo)
        {
            _context = context;
            _checkListRepo = checkListRepo;
        }
        // GET: api/Stats
        [HttpGet("synthese/{type}")]
        public IEnumerable<string> GetStatsSyntheseByType([FromRoute] string type)
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Stats/5
        [HttpGet("synthese/total")]
        public async Task<IActionResult> GetStatsSyntheseByMonth()
        {
            var monthStats = await _context.CheckListRef.Where(w => w.Date.Value.Month == DateTime.Now.Month)
                                               .GroupBy(g => g.Date.Value.Day)
                                               .Select(s => new { label = s.Key, count = s.Count() }).ToListAsync();
            //return _context.CheckListRef.Count();
            return Ok(new { stats = monthStats });
        }

        // GET: api/Stats
        [HttpGet("{site}")]
        public async Task<IActionResult> GetStatsSuspendedCamionsBySite([FromRoute] string site)
        {
            var monthStats = await _context.CheckListRef.Where(w => w.Date.Value.Month == DateTime.Now.Month && w.Etat == true && w.IdSiteNavigation.Libelle == site)
                                               .GroupBy(g => g.Date.Value.Day)
                                               .Select(s => new { label = s.Key, count = s.Count() }).ToListAsync();
            //return _context.CheckListRef.Count();
            return Ok(new { stats = monthStats });
        }
        [HttpGet("NbrTotal")]
        public async Task<IActionResult> GetStatsTotal()
        {
            var totalNonAut = await _context.CheckListRef.Where(g => g.Etat == true)
                 .GroupBy(w => new { w.Etat })
                 .Select(s => new { count = s.Count(), label = "Nonautoriser" }).ToListAsync();
            var totalAut = await _context.CheckListRef.Where(g => g.Etat == false)
                 .GroupBy(w => new { w.Etat })
                 .Select(s => new { count = s.Count(), label = "Autoriser" }).ToListAsync();
            return Ok(new { nonAutoriser = totalNonAut, autoriser = totalAut });
        }

        // GET: api/Stats
        [HttpGet("suspendu/{type}")]
        public async Task<IActionResult> NomberSuspendedCamions(string type)
        {
            /*var a = await _context.CheckListRef.Where(w => w.Etat == true)
                .GroupBy(g => new { g.IdSiteNavigation.Libelle, g.IdVehiculeNavigation.IdEnginNavigation.NomEngin }).
                Select(s => new { label = s.Key.Libelle, type = s.Key.NomEngin, count = s.Count() }).ToListAsync();*/
            var a = await _context.Set<VM_GetCamionByStats>().FromSql($"getEnginByStatus {type}").ToListAsync();
            return new JsonResult(a);
        }

        // GET: api/Stats
        [HttpGet("Nonsuspendu")]
        public async Task<IActionResult> NomberNonSuspendedCamions()
        {
            var a = await _context.CheckListRef.Where(w => w.Etat == false).GroupBy(g => new { g.IdSiteNavigation.Libelle, g.IdVehiculeNavigation.IdEnginNavigation.NomEngin }).Select(s => new { label = s.Key.Libelle, type = s.Key.NomEngin, count = s.Count() }).ToListAsync();
            return Ok(new { stats = a });
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var suspended = await _context.CheckListRef
                .Where(w => w.Etat != null && w.Etat == true && !string.IsNullOrEmpty(w.IdSiteNavigation.Libelle) && !string.IsNullOrEmpty(w.IdVehiculeNavigation.IdEnginNavigation.NomEngin) && new[] { "Plateau", "Benne", "Citerne" }
                .Contains(w.IdVehiculeNavigation.IdEnginNavigation.NomEngin))
                .GroupBy(g => new { g.IdSiteNavigation.Libelle, g.IdVehiculeNavigation.IdEnginNavigation.NomEngin })
                .Select(s => new { label = s.Key.Libelle, type = s.Key.NomEngin, count = s.Count(), etat = "blocked" }).ToListAsync();

            var notSuspended = await _context.CheckListRef
                .Where(w => w.Etat != null && w.Etat == false && !string.IsNullOrEmpty(w.IdSiteNavigation.Libelle) && !string.IsNullOrEmpty(w.IdVehiculeNavigation.IdEnginNavigation.NomEngin) && new[] { "Plateau", "Benne", "Citerne" }
                .Contains(w.IdVehiculeNavigation.IdEnginNavigation.NomEngin))
                .GroupBy(g => new { g.IdSiteNavigation.Libelle, g.IdVehiculeNavigation.IdEnginNavigation.NomEngin })
                .Select(s => new { label = s.Key.Libelle, type = s.Key.NomEngin, count = s.Count(), etat = "notBlocked" }).ToListAsync();

            var listUnion = notSuspended.Union(suspended);

            var sites = _context.Site.Where(w => !String.IsNullOrEmpty(w.Libelle)).Select(x => new SiteDTO { Label = x.Libelle });

            var mapped = listUnion.Select(x =>
            {
                var data = new Stats();
                data.Label = x.label;
                data.Type = x.type;
                if (x.etat == "blocked")
                {
                    data.BlockedCount = x.count;
                }
                else
                {
                    data.NotBlockedCount = x.count;
                }
                return data;
            }).Aggregate(new List<Stats>(), (m, o) =>
            {
                /*var dict = (IDictionary<string, object>)m;
                var dict2 = (IDictionary<string, object>)o;
                System.Diagnostics.Debug.WriteLine("M " + dict["label"]); 
                System.Diagnostics.Debug.WriteLine("O " + dict2["label"]);*/
                // System.Diagnostics.Debug.WriteLine("M " + m[0].Label);
                if (m.Count >= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Count M: " + m.Count);
                    //var eoAsDict = ((IDictionary<string, object>)o);
                    System.Diagnostics.Debug.WriteLine("Type: " + o.Type);
                    /*foreach(var kvp in eoAsDict)
                    {
                        System.Diagnostics.Debug.WriteLine("Property {0} equals {1}", kvp.Key, kvp.Value);
                    }*/
                    var found = m.Find(p =>
                    {
                        // System.Diagnostics.Debug.WriteLine("Find P: " + p.Type + ", O: " + o.Type);
                        return p.Label == o.Label && p.Type == o.Type;
                    });

                    if (found != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Found: " + found.Label);
                        found.BlockedCount = found.BlockedCount != null ? found.BlockedCount + (o.BlockedCount != null ? o.BlockedCount : 0) : (o.BlockedCount != null ? o.BlockedCount : 0);
                        found.NotBlockedCount = found.NotBlockedCount != null ? found.NotBlockedCount + (o.NotBlockedCount != null ? o.NotBlockedCount : 0) : (o.NotBlockedCount != null ? o.NotBlockedCount : 0);
                    }
                    else
                    {
                        o.BlockedCount = o.BlockedCount ?? 0;
                        o.NotBlockedCount = o.NotBlockedCount ?? 0;
                        m.Add(o);
                    }
                }
                return m;
            });

            var benne = Factorization(sites, mapped, "Benne");
            var plateau = Factorization(sites, mapped, "Plateau");
            var citerne = Factorization(sites, mapped, "Citerne");



            return Ok(new { bennes = benne, citernes = citerne, plateaus = plateau });
        }

        // GET: api/Stats
        [HttpGet("controledSite")]
        public async Task<IActionResult> NomberCamionsSite()
        {
            var a = await _context.CheckListRef.GroupBy(g => g.IdSiteNavigation.Libelle).Select(s => new { label = s.Key, count = s.Count() }).ToListAsync();
            return Ok(new { stats = a });
        }
        // GET: api/Stats
        [HttpGet("controled")]
        public int NomberCamion()
        {
            var a = _context.CheckListRef.Where(w => !string.IsNullOrEmpty(w.IdSiteNavigation.Libelle) && !string.IsNullOrEmpty(w.IdVehiculeNavigation.IdEnginNavigation.NomEngin)).Count();
            return a;
        }


        private List<Stats> Factorization(IQueryable<SiteDTO> sites, List<Stats> stats, string type)
        {
            var res = new List<Stats>();
            // var index = 0;
            foreach (var site in sites)
            {
                var stat = new Stats();
                stat.Label = site.Label;
                stat.Type = type;
                System.Diagnostics.Debug.WriteLine("Site P: " + site.Label);
                var found = stats.Find(x => x.Label == site.Label && x.Type == type);
                if (found != null)
                {
                    stat.BlockedCount = found.BlockedCount;
                    stat.NotBlockedCount = found.NotBlockedCount;
                }
                else
                {
                    stat.BlockedCount = 0;
                    stat.NotBlockedCount = 0;
                }
                // index++;
                res.Add(stat);
                System.Diagnostics.Debug.WriteLine("Count P: " + res.Count);
            }
            return res;
        }


        [HttpGet("test2")]
        public Task<int> GetTest()
        {
            var test = _checkListRepo.GetCountControledEngin(4);
            return test;
        }


        [HttpGet("test")]
        public async Task<IActionResult> GetMostErrorControl()
        {
            var most = new List<MostControlError>();
            var checklist = await _checkListRepo.GetAllCheckList();
            var id = 1;

            if (checklist.ToList().Count > 0)
            {
                foreach (var item in checklist)
                {
                    List<bool> engins = new List<bool>();
                    // System.Diagnostics.Debug.WriteLine("Get Type: " + item.CatchAll["checklistEngin"].GetType());
                    if (item.CatchAll["checklistEngin"].GetType().IsGenericType)
                    {

                        System.Diagnostics.Debug.WriteLine("Is Array");
                        IEnumerable enumerable = item.CatchAll["checklistEngin"] as IEnumerable;
                        if (enumerable != null)
                        {
                            // System.Diagnostics.Debug.WriteLine("Is Not Null");

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
                        // System.Diagnostics.Debug.WriteLine("Is Array");
                        IEnumerable enumerable = item.CatchAll["checklistConducteur"] as IEnumerable;
                        if (enumerable != null)
                        {
                            // System.Diagnostics.Debug.WriteLine("Is Not Null");

                            foreach (bool n in enumerable)
                            {
                                conducteurs.Add(n);
                            }
                        }
                    }

                    most.Add(new MostControlError { ID = id, Engins = engins, Conducteurs = conducteurs });
                    id++;
                    if (id == 10)
                        break;
                }
            }

            var indexEngin = 0;
            var indexCond = 0;

            var countIndexEngin0 = 0;
            var countIndexEngin1 = 0;
            var countIndexEngin2 = 0;
            var countIndexEngin3 = 0;
            var countIndexEngin4 = 0;

            var countIndexCond0 = 0;
            var countIndexCond1 = 0;
            var countIndexCond2 = 0;
            var countIndexCond3 = 0;
            var countIndexCond4 = 0;

            var counter = 0;
            // var checkCount = new List<object>();
            var count = new List<object>();
            foreach (var item in most)
            {

                foreach (var engin in item.Engins)
                {
                    if (!engin)
                    {
                        // System.Diagnostics.Debug.WriteLine("Engin: In List: " + counter + ", Index In Array: " + index1);
                        // checkCount.Add(new { indexInList = counter, indexInArray = index1 });
                        switch (indexEngin)
                        {
                            case 0: countIndexEngin0++; break;
                            case 1: countIndexEngin1++; break;
                            case 2: countIndexEngin2++; break;
                            case 3: countIndexEngin3++; break;
                            case 4: countIndexEngin4++; break;
                        }
                        indexEngin++;
                    }

                }

                foreach (var cond in item.Conducteurs)
                {
                    if (!cond)
                    {
                        // System.Diagnostics.Debug.WriteLine("Engin: In List: " + counter + ", Index In Array: " + index1);
                        // checkCount.Add(new { indexInList = counter, indexInArray = index1 });
                        switch (indexCond)
                        {
                            case 0: countIndexCond0++; break;
                            case 1: countIndexCond1++; break;
                            case 2: countIndexCond2++; break;
                            case 3: countIndexCond3++; break;
                            case 4: countIndexCond4++; break;
                        }
                        indexCond++;
                    }

                }
                counter++;
                indexEngin = 0;
                indexCond = 0;
            }

            count.Add(new
            {
                Engin = new
                {
                    controle1 = countIndexEngin0,
                    controle2 = countIndexEngin1,
                    controle3 = countIndexEngin2,
                    controle4 = countIndexEngin3,
                    controle5 = countIndexEngin4
                },
                Cond = new
                {
                    controle1 = countIndexCond0,
                    controle2 = countIndexCond1,
                    controle3 = countIndexCond2,
                    controle4 = countIndexCond3,
                    controle5 = countIndexCond4
                }
            });
            return Ok(count);
        }
    }
    internal class Stats
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public int? BlockedCount { get; set; }
        public int? NotBlockedCount { get; set; }
    }

    internal class SiteDTO
    {
        public string Label { get; set; }
    }

    internal class MostControlError
    {
        public int ID { get; set; }
        public List<bool> Engins { get; set; }
        public List<bool> Conducteurs { get; set; }
    }
}

