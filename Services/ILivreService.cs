using Bibliotheque.Models;

namespace Bibliotheque.Services;

public interface ILivreService
{
    Task<List<Livre>> GetAllAsync(string? searchTerm = null);
    Task<Livre?> GetByIdAsync(string numInventaire);
    Task<bool> ExistsAsync(string numInventaire);
    Task CreateAsync(Livre livre);
    Task<bool> UpdateAsync(Livre livre);
    Task<bool> DeleteAsync(string numInventaire);
}