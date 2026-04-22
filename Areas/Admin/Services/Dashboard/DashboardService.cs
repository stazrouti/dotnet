namespace Bibliotheque.Areas.Admin.Services.Dashboard
{
    using Bibliotheque.Areas.Admin.DTOs;
    using Bibliotheque.Models;
    using ClosedXML.Excel;
    using DocumentFormat.OpenXml.InkML;
    using Microsoft.EntityFrameworkCore;
    using System;
     using ClosedXML.Excel;

    public class DashboardService : IDashboardService
    {
        private readonly BibliothequeContext _context;

        public DashboardService(BibliothequeContext context)
        {
            _context = context;
        }

        public async Task<DashboardVM> GetStatsAsync()
        {
            var now = DateTime.Now;

            var total = await _context.Emprunts.CountAsync();

            var retard = await _context.Emprunts
                .Where(e => e.Estretour == null &&
                            e.Estretour != null &&
                            e.Estretour < now)
                .CountAsync();

            var today = await _context.Emprunts
                .Where(e => e.Dateemprunt == now.Date)
                .CountAsync();

            var last12Months = Enumerable.Range(0, 12)
        .Select(i => now.AddMonths(-i))
        .OrderBy(d => d)
        .ToList();

            var months = last12Months
                .Select(d => d.ToString("MMM yyyy"))
                .ToList();

            var counts = new List<int>();

            foreach (var month in last12Months)
            {
                var count = await _context.Emprunts
                    .Where(e => e.Dateemprunt.Value.Month == month.Month &&
                                e.Dateemprunt.Value.Year == month.Year)
                    .CountAsync();

                counts.Add(count);
            }

            var recent = await _context.Emprunts
    .Include(e => e.CinNavigation)
    .Include(e => e.NuminvNavigation)
    .OrderByDescending(e => e.Dateemprunt)
    .Take(5)
    .ToListAsync();

            return new DashboardVM
            {
                TotalEmprunts = total,
                Retard = retard,
                Today = today,
                Months = months,
                MonthlyCounts = counts,
                RecentEmprunts = recent
            };
        }


public async Task<byte[]> ExportToExcelAsync()
    {
        var query = _context.Emprunts
            .Include(e => e.CinNavigation)
            .Include(e => e.NuminvNavigation)
            .AsQueryable();

        var data = await query.ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reservations");

        // Header
        worksheet.Cell(1, 1).Value = "CIN";
        worksheet.Cell(1, 2).Value = "Étudiant";
        worksheet.Cell(1, 3).Value = "Livre";
        worksheet.Cell(1, 4).Value = "Date Emprunt";
        worksheet.Cell(1, 5).Value = "Date Retour";
        worksheet.Cell(1, 6).Value = "Statut";

        int row = 2;

        foreach (var item in data)
        {
            var statut = "En cours";

            if (item.Estretour != null)
                statut = "Retourné";
            else if (item.Dateretour != null && item.Dateretour < DateTime.Now)
                statut = "En retard";

            worksheet.Cell(row, 1).Value = item.Cin;
            worksheet.Cell(row, 2).Value = $"{item.CinNavigation.Nom}";
            worksheet.Cell(row, 3).Value = item.NuminvNavigation.Titre;
            worksheet.Cell(row, 4).Value = item.Dateemprunt;
            worksheet.Cell(row, 5).Value = item.Estretour;
            worksheet.Cell(row, 6).Value = statut;

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
}
