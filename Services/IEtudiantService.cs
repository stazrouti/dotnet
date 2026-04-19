using Bibliotheque.Models;

namespace Bibliotheque.Services;

public interface IEtudiantService
{
    Task<IEnumerable<Etudiant>> GetAllAsync(string? searchTerm = null);
    Task<Etudiant?> GetByIdAsync(string cin);
    Task<Etudiant?> GetByMatriculeAsync(string matricule);
    Task<bool> ExistsAsync(string cin);
    Task<bool> MatriculeExistsAsync(string matricule, string? excludeCin = null);
    Task<bool> EmailExistsAsync(string email, string? excludeCin = null);
    Task<bool> CreateAsync(Etudiant etudiant);
    Task<bool> UpdateAsync(Etudiant etudiant);
    Task<bool> DeleteAsync(string cin);
    Task<IEnumerable<Emprunt>> GetActiveLoansAsync(string cin);
    Task<IEnumerable<Visite>> GetRecentVisitesAsync(string cin, int days = 30);
    Task<Etudiant?> GetVerifiedEtudiantForUserAsync(string? email);
    Task<int> CountCurrentReservationsAsync(string cin);
}
