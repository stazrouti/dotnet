using Bibliotheque.Models;

namespace Bibliotheque.Areas.Admin.DTOs
{
    public class DashboardVM
    {
        public int TotalEmprunts { get; set; }
        public int Retard { get; set; }
        public int Today { get; set; }

        public List<string> Months { get; set; }
        public List<int> MonthlyCounts { get; set; }

        public List<Emprunt> RecentEmprunts { get; set; }
    }
}
