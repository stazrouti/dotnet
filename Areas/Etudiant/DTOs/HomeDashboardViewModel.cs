namespace Bibliotheque.Areas.Etudiant.DTOs;

public class HomeDashboardViewModel
{
    public bool ShowEtudiantSummary { get; set; }
    public int TodayReservationsCount { get; set; }
    public int DueTodayCount { get; set; }
    public int DueThisWeekCount { get; set; }
    public int CurrentReservationCount { get; set; }
    public int MaxReservationCount { get; set; } = 3;
    public int RemainingQuota => Math.Max(0, MaxReservationCount - CurrentReservationCount);
}