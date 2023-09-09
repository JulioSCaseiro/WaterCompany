using System.ComponentModel.DataAnnotations;

namespace WaterCompanyWeb.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
