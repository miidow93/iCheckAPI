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

        public int Idrole { get; set; }
        public string Role1 { get; set; }

        public ICollection<Users> Users { get; set; }
    }
}
