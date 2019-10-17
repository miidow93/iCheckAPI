using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<Users>();
        }

        public int Id { get; set; }
        public string Libelle { get; set; }

        public ICollection<Users> Users { get; set; }
    }
}
