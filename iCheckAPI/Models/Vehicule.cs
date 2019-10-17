using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Vehicule
    {
        public int Id { get; set; }
        public string Matricule { get; set; }
        public int? IdEngin { get; set; }

        public Engins IdEnginNavigation { get; set; }
    }
}
