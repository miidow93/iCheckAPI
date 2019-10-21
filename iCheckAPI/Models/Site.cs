using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Site
    {
        public Site()
        {
            CheckListRef = new HashSet<CheckListRef>();
            Users = new HashSet<Users>();
        }

        public int Id { get; set; }
        public string Libelle { get; set; }

        public ICollection<CheckListRef> CheckListRef { get; set; }
        public ICollection<Users> Users { get; set; }
    }
}
