using System.ComponentModel.DataAnnotations;

namespace WaterCompanyWeb.Data.Entities
{
    public class Client : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MinLength(9, ErrorMessage = "Incorrect phone number. (Number too short)")]
        [MaxLength(9, ErrorMessage = "Incorrect phone number. (Number too big)")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
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

        public User User { get; set; }
    }
}
