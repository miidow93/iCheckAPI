using iCheckAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCheckAPI.Repositories
{
    public class VehiculeRepo : IVehiculeRepo
    {
        private readonly ICheckContext _context;
        public VehiculeRepo(ICheckContext context)
        {
            _context = context;
        }
        public bool GetVehiculeByMatricule(string matricule)
        {
            return _context.Vehicule.Any(x => x.Matricule == matricule);
        }

        public int GetEnginByName(string nom)
        {
            var engin = _context.Engins.FirstOrDefault(x => x.NomEngin.ToLower() == nom.ToLower());
            if(engin == null)
            {
                Engins engins = new Engins()
                {
                    NomEngin = nom
                };
                _context.Engins.Add(engins);
                _context.SaveChanges();
                var newID = new { id = engins.Id };
                return newID.id;
            }
            return engin.Id;
        }

        public async Task Create(Vehicule vehicule)
        {
            _context.Vehicule.Add(vehicule);
            await _context.SaveChangesAsync();
        }
    }

    public interface IVehiculeRepo
    {
        bool GetVehiculeByMatricule(string matricule);

        int GetEnginByName(string nom);

        Task Create(Vehicule vehicule);
    }
}
