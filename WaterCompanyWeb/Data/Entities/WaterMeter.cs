using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace WaterCompanyWeb.Data.Entities
{
    public class WaterMeter
    {
        public int Id { get; set; }

        public int Rank { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Consumption Date")]
        public DateTime ConsumptionDate { get; set; }

        public decimal Value { get; set; }

        [Display(Name = "Total Consumption")]
        public decimal TotalConsumption { get; set; }
    }
}
