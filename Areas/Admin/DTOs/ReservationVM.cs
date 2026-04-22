using Bibliotheque.Models;

namespace Bibliotheque.Areas.Admin.DTOs
{
        public class ReservationVM
        {
            public List<Emprunt> Reservations { get; set; }

            public string SearchTerm { get; set; }

            public int PageNumber { get; set; }
            public int TotalPages { get; set; }
        }
}
