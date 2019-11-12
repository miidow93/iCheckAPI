using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iCheckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {

        // synthese mensuel des camions par type
        // synthese mensuel des camions.
        // nombre des camions suspendus par site + nombre total des camions controlés


        // GET: api/Stats
        [HttpGet("synthese/{type}")]
        public IEnumerable<string> GetStatsSyntheseByType([FromRoute] string type)
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Stats/5
        [HttpGet("synthese/total")]
        public string GetStatsSynthese()
        {
            return "value";
        }

        // POST: api/Stats
        [HttpGet("{site}")]
        public void GetStatsSuspendedCamionsBySite([FromRoute] string site)
        {
        }
    }
}
