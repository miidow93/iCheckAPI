using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class CheckListRef
    {
        public int Id { get; set; }
        public string IdCheckListRef { get; set; }
        public int? IdSite { get; set; }
        public int? IdConducteur { get; set; }
        public int? IdVehicule { get; set; }
        public DateTime? Date { get; set; }
        public double? Rating { get; set; }
        public bool? Etat { get; set; }

        public Conducteur IdConducteurNavigation { get; set; }
        public Site IdSiteNavigation { get; set; }
        public Vehicule IdVehiculeNavigation { get; set; }
    }
}
