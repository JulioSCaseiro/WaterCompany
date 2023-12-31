﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WaterCompanyWeb.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Confirm Email")]
        [DataType(DataType.EmailAddress)]
        [Compare("Username")]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string Confirm { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber{ get; set; }
        [Required]
        public string Address { get; set; }

        [Required]
        public string NIF { get; set; }

        [Required]
        public string ZIP { get; set; }

        [Required(ErrorMessage = "Role required")]
        [Display(Name = "Role")]
        public string AccountRole { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
