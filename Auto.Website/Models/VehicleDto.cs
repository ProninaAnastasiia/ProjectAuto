using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace Auto.Website.Models;

public class VehicleDto
{
    private string registration;

    public string ModelName { get; set; }

    [HiddenInput(DisplayValue = false)] public string ModelCode { get; set; }

    [Required]
    [DisplayName("Registration Plate")]
    public string Registration
    {
        get => NormalizeRegistration(registration);
        set => registration = value;
    }

    [Required]
    [DisplayName("Year of first registration")]
    [Range(1950, 2022)]
    public int Year { get; set; }

    [Required] [DisplayName("Colour")] public string Color { get; set; }

    private static string NormalizeRegistration(string reg)
    {
        return reg == null ? reg : Regex.Replace(reg.ToUpperInvariant(), "[^A-Z0-9]", "");
    }
}