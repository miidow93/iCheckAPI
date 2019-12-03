using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCheckAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/Stats
        [HttpGet("suspendu")]
        public async Task<IActionResult> NomberSuspendedCamions()
        {
            var a = await _context.CheckListRef.Where(w => w.Etat == true).GroupBy(g => new { g.IdSiteNavigation.Libelle, g.IdVehiculeNavigation.IdEnginNavigation.NomEngin }).Select(s => new { label = s.Key.Libelle, type = s.Key.NomEngin, count = s.Count() }).ToListAsync();
            return Ok(new { stats = a });
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
                .Where(w => w.Etat == true && new[] { "Plateau", "Benne", "Citerne" }.Contains(w.IdVehiculeNavigation.IdEnginNavigation.NomEngin))
                .GroupBy(g => new { g.IdSiteNavigation.Libelle, g.IdVehiculeNavigation.IdEnginNavigation.NomEngin })
                .Select(s => new { label = s.Key.Libelle, type = s.Key.NomEngin, count = s.Count(), etat = "blocked" }).ToListAsync();
            var notSuspended = await _context.CheckListRef
                .Where(w => w.Etat == false && new[] { "Plateau", "Benne", "Citerne" }
                .Contains(w.IdVehiculeNavigation.IdEnginNavigation.NomEngin))
                .GroupBy(g => new { g.IdSiteNavigation.Libelle, g.IdVehiculeNavigation.IdEnginNavigation.NomEngin })
                .Select(s => new { label = s.Key.Libelle, type = s.Key.NomEngin, count = s.Count(), etat = "notBlocked" }).ToListAsync();

            var listUnion = notSuspended.Union(suspended);

            return Ok(new { stats = listUnion });
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
    }
}
