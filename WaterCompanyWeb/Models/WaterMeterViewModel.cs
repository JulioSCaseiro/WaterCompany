using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Models
{
    public class WaterMeterViewModel : WaterMeter
    {
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        public IEnumerable<SelectListItem> Clients { get; set; }
    }
}
