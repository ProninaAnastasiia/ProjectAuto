using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace Auto.Website.Models;

public class OwnerDto
{
    private string vehicleRegistration;

    [Required] [DisplayName("First Name")] public string FirstName { get; set; }

    [Required] [DisplayName("Last Name")] public string LastName { get; set; }

    [Required]
    [DisplayName("Phone Number")]
    public string PhoneNumber { get; set; }

    [HiddenInput(DisplayValue = false)]
    public string VehicleCode
    {
        get => NormalizeRegistration(vehicleRegistration);
        set => vehicleRegistration = value;
    }

    [Required] [DisplayName("Email")] public string Email { get; set; }

    private static string NormalizeRegistration(string reg)
    {
        return reg == null ? reg : Regex.Replace(reg.ToUpperInvariant(), "[^A-Z0-9]", "");
    }
}