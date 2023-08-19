using System.ComponentModel.DataAnnotations;

namespace WaterCompanyWeb.Data.Entities
{
    public class Client : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters lenght.")]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [MinLength(9, ErrorMessage = "Incorrect phone number. (Number too short)")]
        [MaxLength(9, ErrorMessage = "Incorrect phone number. (Number too big)")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}(-\d{3})?$", ErrorMessage = "Invalid Zip code!")]
        [Display(Name = "Zip code")]
        public string PostalCode { get; set; }

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

        public User User { get; set; }
    }
}
