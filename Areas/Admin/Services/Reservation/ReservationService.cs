namespace Bibliotheque.Areas.Admin.Services.Reservation
{
    using Bibliotheque.Areas.Admin.DTOs;
    using Bibliotheque.Models;
    using Microsoft.EntityFrameworkCore;
    using System;

    public class ReservationService : IReservationService
    {
        private readonly BibliothequeContext _context;

        public ReservationService(BibliothequeContext context)
        {
            _context = context;
        }

        public async Task<ReservationVM> GetPagedAsync(string search, int page, int pageSize)
        {
            var query = _context.Emprunts
                .Include(e => e.CinNavigation)
                .Include(e => e.NuminvNavigation)
                .AsQueryable();

            // 🔍 SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e =>
                    e.Cin.Contains(search) ||
                    e.NuminvNavigation.Titre.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(e => e.Dateemprunt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ReservationVM
            {
                Reservations = data,
                PageNumber = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                SearchTerm = search
            };
        }

        // 🔥 Cancel with deadline protection
        public async Task CancelAsync(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);

            if (emprunt != null)
            {
                bool isLate = emprunt.Estretour != null && emprunt.Estretour < DateTime.Now;

                if (emprunt.CanCancel && !isLate)
                {
                    _context.Emprunts.Remove(emprunt);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task MarkAsReturnedAsync(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);

            if (emprunt != null)
            {
                emprunt.Estretour = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAsConfirmedAsync(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);

            if (emprunt != null)
            {
                emprunt.CanCancel = false;

                await _context.SaveChangesAsync();
            }
        }
    }
}
