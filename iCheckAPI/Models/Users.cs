using System;
using System.Collections.Generic;

namespace iCheckAPI.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string NomComplet { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? IdRole { get; set; }

        public Role IdRoleNavigation { get; set; }
    }
}
