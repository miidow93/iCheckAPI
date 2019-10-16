using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Conducteur
    {
        public int Id { get; set; }
        public string NomComplet { get; set; }
        public string Cin { get; set; }
        public string Cnss { get; set; }
        public string Assurance { get; set; }
        public DateTime? DateValiditeAssurance { get; set; }
        public string Patente { get; set; }
        public int? IdSociete { get; set; }

        public Societe IdsocieteNavigation { get; set; }
    }
}
