using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Auto.Website.Models {
    public class OwnerDto {

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        
        private string vehicleRegistration;

        private static string NormalizeRegistration(string reg) {
            return reg == null ? reg : Regex.Replace(reg.ToUpperInvariant(), "[^A-Z0-9]", "");
        }
        
        [HiddenInput(DisplayValue = false)]
        public string VehicleCode {
            get => NormalizeRegistration(vehicleRegistration);
            set => vehicleRegistration = value;
        }
        
        [Required]
        [DisplayName("Email")]
        public string Email { get; set; }
    }
}