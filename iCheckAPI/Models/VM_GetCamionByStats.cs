using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace iCheckAPI.Models
{
    public class VM_GetCamionByStats
    {
        [Key]
        public string libelle { get; set; }
        public int bloque { get; set; }
        public int Nonbloque { get; set; }
    }
}
