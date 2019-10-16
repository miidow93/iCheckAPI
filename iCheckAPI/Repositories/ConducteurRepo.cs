﻿using iCheckAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCheckAPI.Repositories
{
    public class ConducteurRepo : IConducteurRepo
    {
        private readonly ICheckContext _context;
        public ConducteurRepo(ICheckContext context)
        {
            _context = context;
        }
        public bool GetConducteurByCIN(string cin)
        {
            return _context.Conducteur.Any(x => x.Cin == cin);
        }

        public async Task Create(Conducteur conducteur)
        {
            _context.Conducteur.Add(conducteur);
            await _context.SaveChangesAsync();
        }


    }

    public interface IConducteurRepo
    {
        bool GetConducteurByCIN(string cin);

        Task Create(Conducteur conducteur);
    }
}
