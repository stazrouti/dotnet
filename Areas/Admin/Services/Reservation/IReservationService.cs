using Bibliotheque.Areas.Admin.DTOs;

namespace Bibliotheque.Areas.Admin.Services.Reservation
{
    public interface IReservationService
    {
        Task<ReservationVM> GetPagedAsync(string search, int page, int pageSize);
        Task CancelAsync(int id);
        Task MarkAsReturnedAsync(int id);

        Task MarkAsConfirmedAsync(int id);
    }
}
