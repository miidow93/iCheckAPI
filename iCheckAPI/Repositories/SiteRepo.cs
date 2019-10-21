using iCheckAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCheckAPI.Repositories
{
    public class SiteRepo : ISiteRepo
    {
        private readonly ICheckContext _context;
        public SiteRepo(ICheckContext context)
        {
            _context = context;
        }
        public Site GetSiteByLibelle(string matricule)
        {
            return _context.Site.FirstOrDefault(x => x.Libelle.ToLower() == matricule.ToLower());
        }

        public async Task Create(Site site)
        {
            _context.Site.Add(site);
            await _context.SaveChangesAsync();
        }
    }

    public interface ISiteRepo
    {
        Site GetSiteByLibelle(string matricule);

        Task Create(Site site);
    }
}
