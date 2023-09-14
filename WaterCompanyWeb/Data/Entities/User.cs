using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WaterCompanyWeb.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        [RegularExpression(@"^\d{4}(-\d{3})?$", ErrorMessage = "Invalid Zip code!")]
        [Display(Name = "Zip code")]
        public string ZIPCode { get; set; }

        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        [Display(Name = "Tax identification number")]
        public string NIF { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }
                return $"https://localhost:44311{ImageUrl.Substring(1)}";
            }
        }

        [Required]
        [Display(Name = "Active")]
        public bool IsAvailable { get; set; }
    }
}
