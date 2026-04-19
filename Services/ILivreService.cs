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
    Task<List<Emprunt>> GetReservationsForBookAsync(string numInventaire, DateTime fromDate, DateTime toDate);
    Task<bool> IsBookAvailableAsync(string numInventaire, DateTime startDate, DateTime endDate);
    Task CreateReservationAsync(string cin, string numInventaire, DateTime startDate, DateTime endDate);
    Task<Emprunt?> GetReservationByIdAsync(decimal reservationId);
    Task<bool> CancelReservationAsync(decimal reservationId, string cin);
}