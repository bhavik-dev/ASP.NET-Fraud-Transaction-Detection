using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2Delivery2.DataAccessLayer.Models
{
    internal class RoleUsersViewModel
    {
        public string RoleName { get; set; }
        public List<ApplicationUser> UsersInRole { get; set; }
        public List<ApplicationUser> AvailableUsers { get; set; }
    }
}
