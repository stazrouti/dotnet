namespace Bibliotheque.Models;

public class AdminUsersViewModel
{
    public List<AdminUserRolesViewModel> Users { get; set; } = new();
    public List<string> AvailableRoles { get; set; } = new();
}