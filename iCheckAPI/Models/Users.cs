using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Users
    {
        public Users()
        {
            CheckListRef = new HashSet<CheckListRef>();
        }

        public int Id { get; set; }
        public string NomComplet { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? IdRole { get; set; }
        public int? IdSite { get; set; }

        public Role IdRoleNavigation { get; set; }
        public Site IdSiteNavigation { get; set; }
        public ICollection<CheckListRef> CheckListRef { get; set; }
    }
}
