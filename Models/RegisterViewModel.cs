using System.ComponentModel.DataAnnotations;

namespace Bibliotheque.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Le nom complet est obligatoire")]
    [StringLength(150)]
    public string NomComplet { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire")]
    [EmailAddress(ErrorMessage = "Email invalide")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Minimum 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmation est obligatoire")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "La confirmation ne correspond pas")]
    public string ConfirmPassword { get; set; } = string.Empty;
}