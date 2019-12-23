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

        public StatsController(ICheckContext context)
        {
            _context = context;
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
            var totalAut = await _context.CheckListRef.Where(g=>g.Etat == false)
                 .GroupBy(w => new { w.Etat })
                 .Select(s => new { count = s.Count(), label = "Autoriser" }).ToListAsync();
            return Ok(new { nonAutoriser = totalNonAut , autoriser = totalAut});
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
            var a = _context.CheckListRef.Count();
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
}
