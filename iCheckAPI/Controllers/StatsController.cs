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

        // GET: api/Stats/5
        [HttpGet("synthese/totals")]
        public string GetStatscamions()
        {
            return "value";
        }

        // POST: api/Stats
        [HttpGet("{site}")]
        public async Task<IActionResult> GetStatsSuspendedCamionsBySite()
        {
            var monthStats = await _context.CheckListRef.Where(w => w.Date.Value.Month == DateTime.Now.Month && w.Etat == true)
                                               .GroupBy(g => g.Date.Value.Day)
                                               .Select(s => new { label = s.Key, count = s.Count() }).ToListAsync();
            //return _context.CheckListRef.Count();
            return Ok(new { stats = monthStats });
        }
    }
}
