using System.ComponentModel.DataAnnotations;

namespace Bibliotheque.Models;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Le mot de passe actuel est obligatoire")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nouveau mot de passe est obligatoire")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Minimum 8 caracteres")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmation est obligatoire")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "La confirmation ne correspond pas")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}