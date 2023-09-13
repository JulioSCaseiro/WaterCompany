using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace WaterCompanyWeb.Data.Entities
{
    public class WaterMeter : IEntity
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Consumption Date")]
        public DateTime ConsumptionDate { get; set; }

        public double Value { get; set; }

        [Display(Name = "Total Consumption")]
        public double TotalConsumption { get; set; }

        public Client Client { get; set; }
    }
}
