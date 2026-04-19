using Bibliotheque.Models;

namespace Bibliotheque.Services;

public interface ILivreService
{
    Task<List<Livre>> GetAllAsync(string? searchTerm = null);
    Task<(List<Livre> Livres, int TotalItems)> GetPagedAsync(string? searchTerm, int page, int pageSize);
    Task<Livre?> GetByIdAsync(string numInventaire);
    Task<bool> ExistsAsync(string numInventaire);
    Task CreateAsync(Livre livre);
    Task<bool> UpdateAsync(Livre livre);
    Task<bool> DeleteAsync(string numInventaire);
    Task<List<Emprunt>> GetReservationsForBookAsync(string numInventaire, DateTime fromDate, DateTime toDate);
    Task<List<Emprunt>> GetReservationsForStudentAsync(string cin);
    Task<bool> IsBookAvailableAsync(string numInventaire, DateTime startDate, DateTime endDate);
    Task CreateReservationAsync(string cin, string numInventaire, DateTime startDate, DateTime endDate);
    Task<Emprunt?> GetReservationByIdAsync(decimal reservationId);
    Task<bool> CancelReservationAsync(decimal reservationId, string cin);
    
    // Review/Rating methods
    Task<bool> CanReviewBookAsync(string cin, string numInventaire);
    Task CreateReviewAsync(string cin, string numInventaire, int note, string? commentaire);
    Task<BookReviewsViewModel> GetReviewsForBookAsync(string numInventaire, string? currentUserCin = null);
    Task<AvisViewModel?> GetReviewByIdAsync(decimal reviewId);
    Task<bool> UpdateReviewAsync(decimal reviewId, string cin, int note, string? commentaire);
    Task<bool> DeleteReviewAsync(decimal reviewId, string cin);
}