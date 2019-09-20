using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCheckAPI.Models
{
    public class VmConducteur
    {
        public string NomComplet { get; set; }

        public string Cin { get; set; }

        public string Cnss { get; set; }

        public string Assurance { get; set; }
        public DateTime DateValiditeAssurance { get; set; }
        public string Patente { get; set; }
        public string Societe { get; set; }
        


    }
}
