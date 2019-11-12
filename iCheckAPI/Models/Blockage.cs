using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Blockage
    {
        public int Id { get; set; }
        public int? IdVehicule { get; set; }
        public DateTime? DateBlockage { get; set; }
        public string Motif { get; set; }
        public DateTime? DateDeblockage { get; set; }
        public string ImageUrl { get; set; }
        public string IdCheckList { get; set; }

        public Vehicule IdVehiculeNavigation { get; set; }
    }
}
