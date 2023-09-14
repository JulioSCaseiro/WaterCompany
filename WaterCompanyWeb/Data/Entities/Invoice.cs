using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace WaterCompanyWeb.Data.Entities
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        [Display(Name = "Water Meter Counter")]
        public WaterMeter WaterMeter { get; set; }
    }
}
