namespace Bibliotheque.Models;

public class EtudiantIndexViewModel
{
    public string? Search { get; set; }
    public List<Etudiant> Etudiants { get; set; } = new();
}

public class EtudiantDetailsViewModel
{
    public Etudiant Etudiant { get; set; } = null!;
    public List<Emprunt> ActiveLoans { get; set; } = new();
    public List<Visite> RecentVisits { get; set; } = new();
}

public class EtudiantReservationRowViewModel
{
    public decimal ReservationId { get; set; }
    public string Numinventaire { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public string Auteur { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReservationStatus Status { get; set; }
    public bool CanCancel { get; set; }
}

public class EtudiantReservationStatsViewModel
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Upcoming { get; set; }
    public int Returned { get; set; }
    public int Cancelled { get; set; }
}

public enum EtudiantDashboardAlertLevel
{
    Info,
    Warning,
    Danger
}

public class EtudiantDashboardAlertViewModel
{
    public EtudiantDashboardAlertLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Numinventaire { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class EtudiantReservationDashboardViewModel
{
    public Etudiant Etudiant { get; set; } = null!;
    public EtudiantReservationStatsViewModel Stats { get; set; } = new();
    public int TodayReservationsCount { get; set; }
    public int DueTodayCount { get; set; }
    public int DueThisWeekCount { get; set; }
    public int CurrentReservationCount { get; set; }
    public int MaxReservationCount { get; set; } = 3;
    public int RemainingQuota => Math.Max(0, MaxReservationCount - CurrentReservationCount);
    public List<EtudiantDashboardAlertViewModel> Alerts { get; set; } = new();
    public List<EtudiantReservationRowViewModel> CurrentReservations { get; set; } = new();
    public List<EtudiantReservationRowViewModel> HistoryReservations { get; set; } = new();
}
