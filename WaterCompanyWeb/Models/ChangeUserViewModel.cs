using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Models
{
    public class ChangeUserViewModel : User
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
