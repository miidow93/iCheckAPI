using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Societe
    {
        public Societe()
        {
            Conducteur = new HashSet<Conducteur>();
        }

        public int IdSociete { get; set; }
        public string Libelle { get; set; }

        public ICollection<Conducteur> Conducteur { get; set; }
    }
}
