namespace Bibliotheque.Models;

public enum ReservationStatus
{
    Reserved,
    Active,
    Returned,
    Cancelled
}

public class ReservationItemViewModel
{
    public decimal Id { get; set; }
    public string Cin { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReservationStatus Status { get; set; }
    public bool CanCancel { get; set; }
}

public class LivreDetailsViewModel
{
    public Livre Livre { get; set; } = new();
    public List<ReservationItemViewModel> Reservations { get; set; } = new();
    public bool CanReserve { get; set; }
    public string? ReservationBlockedReason { get; set; }
    public int CurrentReservationCount { get; set; }
    public int MaxReservationCount { get; set; } = 3;
    public BookReviewsViewModel Reviews { get; set; } = new();
}

public class LivreReserveViewModel
{
    public string Numinventaire { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public DateOnly? BookDateArrivage { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public List<ReservationItemViewModel> Reservations { get; set; } = new();
    public int CurrentReservationCount { get; set; }
    public int MaxReservationCount { get; set; } = 3;
}
