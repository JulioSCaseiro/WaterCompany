﻿using System.ComponentModel.DataAnnotations;

namespace WaterCompanyWeb.Data.Entities
{
    public class Client : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters lenght.")]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [MinLength(9, ErrorMessage = "Not a valide phone number!")]
        [MaxLength(9, ErrorMessage = "Not a valide phone number!")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}(-\d{3})?$", ErrorMessage = "Invalid Postal Code!")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        [Display(Name = "Tax identification number")]
        public string NIF { get; set; }

        [Required]
        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }
    }
}