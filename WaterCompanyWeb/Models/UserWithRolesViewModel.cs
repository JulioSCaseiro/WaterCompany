using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Models
{
    public class UserWithRolesViewModel
    {
        public List<string> Roles { get; set; }

        public User User { get; set; }

        public string UserListUrl { get; set; }
    }
}
