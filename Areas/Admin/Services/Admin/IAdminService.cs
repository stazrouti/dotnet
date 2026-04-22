using Bibliotheque.Models;

namespace Bibliotheque.Areas.Admin.Services.Admin
{
    public interface IAdminService
    {
            Task<Boolean> UpdateEtudiant(string? searchTerm = null);

            Task<Boolean> AddEtudiant(User user);

    }
}
