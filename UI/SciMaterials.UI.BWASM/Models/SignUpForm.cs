using System.ComponentModel.DataAnnotations;

namespace SciMaterials.UI.BWASM.Models;

public class SignUpForm
{
    [Required]
    [StringLength(255, ErrorMessage = "Name length can't be more than 255.")]
    public string? Username { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
    public string? Password { get; set; }
}