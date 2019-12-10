using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string NomComplet { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? Idrole { get; set; }
        public int? IdSite { get; set; }

        public Site IdSiteNavigation { get; set; }
        public Role IdroleNavigation { get; set; }
    }
}
