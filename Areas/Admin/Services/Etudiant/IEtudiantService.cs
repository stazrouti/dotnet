using Bibliotheque.Models;

namespace Bibliotheque.Areas.Admin.Services.Etudiant;

public interface IEtudiantService
{
    Task<IEnumerable<Models.Etudiant>> GetAllAsync(string? searchTerm = null);
    Task<Models.Etudiant?> GetByIdAsync(string cin);
    Task<Models.Etudiant?> GetByMatriculeAsync(string matricule);
    Task<bool> ExistsAsync(string cin);
    Task<bool> MatriculeExistsAsync(string matricule, string? excludeCin = null);
    Task<bool> EmailExistsAsync(string email, string? excludeCin = null);
    Task<bool> CreateAsync(Models.Etudiant etudiant);
    Task<bool> UpdateAsync(Models.Etudiant etudiant);
    Task<bool> DeleteAsync(string cin);
    Task<IEnumerable<Emprunt>> GetActiveLoansAsync(string cin);
    Task<IEnumerable<Visite>> GetRecentVisitesAsync(string cin, int days = 30);
    Task<Models.Etudiant?> GetVerifiedEtudiantForUserAsync(string? email);
    Task<int> CountCurrentReservationsAsync(string cin);
}
