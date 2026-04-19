using Bibliotheque.Models;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheque.Services;

public class LivreService : ILivreService
{
    private readonly BibliothequeContext _context;

    public LivreService(BibliothequeContext context)
    {
        _context = context;
    }

    public async Task<List<Livre>> GetAllAsync(string? searchTerm = null)
    {
        var query = _context.Livres.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(l =>
                (l.Numinventaire != null && l.Numinventaire.Contains(term)) ||
                (l.Titre != null && l.Titre.Contains(term)) ||
                (l.Auteur != null && l.Auteur.Contains(term)) ||
                (l.Isbn != null && l.Isbn.Contains(term)));
        }

        return await query
            .OrderBy(l => l.Titre)
            .ThenBy(l => l.Numinventaire)
            .ToListAsync();
    }

    public async Task<Livre?> GetByIdAsync(string numInventaire)
    {
        return await _context.Livres
            .Include(l => l.Motcles)
            .Include(l => l.Emprunts)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Numinventaire == numInventaire);
    }

    public async Task<bool> ExistsAsync(string numInventaire)
    {
        return await _context.Livres.AnyAsync(l => l.Numinventaire == numInventaire);
    }

    public async Task CreateAsync(Livre livre)
    {
        livre.CreatedAt = DateTime.UtcNow;
        livre.UpdatedAt = DateTime.UtcNow;

        _context.Livres.Add(livre);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Livre livre)
    {
        var existing = await _context.Livres.FirstOrDefaultAsync(l => l.Numinventaire == livre.Numinventaire);
        if (existing is null)
        {
            return false;
        }

        existing.Isbn = livre.Isbn;
        existing.Titre = livre.Titre;
        existing.Auteur = livre.Auteur;
        existing.Editeur = livre.Editeur;
        existing.Datepublication = livre.Datepublication;
        existing.Datearrivage = livre.Datearrivage;
        existing.Résumé = livre.Résumé;
        existing.Cote = livre.Cote;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string numInventaire)
    {
        var livre = await _context.Livres.FirstOrDefaultAsync(l => l.Numinventaire == numInventaire);
        if (livre is null)
        {
            return false;
        }

        _context.Livres.Remove(livre);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Emprunt>> GetReservationsForBookAsync(string numInventaire, DateTime fromDate, DateTime toDate)
    {
        var from = fromDate.Date;
        var to = toDate.Date;

        return await _context.Emprunts
            .Where(e => e.Numinv == numInventaire &&
                        !e.Estretour.HasValue &&
                        e.Dateemprunt.HasValue &&
                        (e.Dateemprunt.Value.Date <= to) &&
                        (!e.Dateretour.HasValue || e.Dateretour.Value.Date >= from))
            .OrderBy(e => e.Dateemprunt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> IsBookAvailableAsync(string numInventaire, DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        var hasConflict = await _context.Emprunts.AnyAsync(e =>
            e.Numinv == numInventaire &&
            !e.Estretour.HasValue &&
            e.Dateemprunt.HasValue &&
            e.Dateemprunt.Value.Date <= end &&
            (!e.Dateretour.HasValue || e.Dateretour.Value.Date >= start));

        return !hasConflict;
    }

    public async Task CreateReservationAsync(string cin, string numInventaire, DateTime startDate, DateTime endDate)
    {
        var reservation = new Emprunt
        {
            Cin = cin,
            Numinv = numInventaire,
            Dateemprunt = startDate.Date,
            Dateretour = endDate.Date,
            Estretour = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Emprunts.Add(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<Emprunt?> GetReservationByIdAsync(decimal reservationId)
    {
        return await _context.Emprunts
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == reservationId);
    }

    public async Task<bool> CancelReservationAsync(decimal reservationId, string cin)
    {
        var reservation = await _context.Emprunts.FirstOrDefaultAsync(e => e.Id == reservationId);
        if (reservation is null)
        {
            return false;
        }

        if (!string.Equals(reservation.Cin, cin, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (reservation.Estretour.HasValue)
        {
            return false;
        }

        reservation.Estretour = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}