using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Models
{
    public class UserWithRolesViewModel : User
    {
        public string UserId { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
