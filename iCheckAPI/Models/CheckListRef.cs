using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class CheckListRef
    {
        public int Id { get; set; }
        public string IdCheckListRef { get; set; }
        public int? IdConducteur { get; set; }
        public int? IdVehicule { get; set; }
        public DateTime? Date { get; set; }

        public Conducteur IdConducteurNavigation { get; set; }
        public Vehicule IdVehiculeNavigation { get; set; }
    }
}
