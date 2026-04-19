namespace Bibliotheque.Models;

public class AdminUserRolesViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}