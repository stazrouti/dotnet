using System.ComponentModel.DataAnnotations;

namespace Bibliotheque.Models;

public class ProfileViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom complet est obligatoire")]
    [StringLength(150)]
    public string NomComplet { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}