﻿using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Engins
    {
        public Engins()
        {
            Vehicule = new HashSet<Vehicule>();
        }

        public int Id { get; set; }
        public string NomEngin { get; set; }
        public string ImageEngin { get; set; }

        public ICollection<Vehicule> Vehicule { get; set; }
    }
}
