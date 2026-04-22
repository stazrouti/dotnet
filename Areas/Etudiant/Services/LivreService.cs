using Bibliotheque.Areas.Etudiant.DTOs;
using Bibliotheque.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheque.Areas.Etudiant.Services;

[Area("Etudiant")]
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
                l.Numinventaire != null && l.Numinventaire.Contains(term) ||
                l.Titre != null && l.Titre.Contains(term) ||
                l.Auteur != null && l.Auteur.Contains(term) ||
                l.Isbn != null && l.Isbn.Contains(term));
        }

        return await query
            .OrderBy(l => l.Titre)
            .ThenBy(l => l.Numinventaire)
            .ToListAsync();
    }

    public async Task<(List<Livre> Livres, int TotalItems)> GetPagedAsync(string? searchTerm, int page, int pageSize)
    {
        var query = _context.Livres.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(l =>
                l.Numinventaire != null && l.Numinventaire.Contains(term) ||
                l.Titre != null && l.Titre.Contains(term) ||
                l.Auteur != null && l.Auteur.Contains(term) ||
                l.Isbn != null && l.Isbn.Contains(term));
        }

        var totalItems = await query.CountAsync();

        var livres = await query
            .OrderBy(l => l.Titre)
            .ThenBy(l => l.Numinventaire)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (livres, totalItems);
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
                        e.Dateemprunt.Value.Date <= to &&
                        (!e.Dateretour.HasValue || e.Dateretour.Value.Date >= from))
            .OrderBy(e => e.Dateemprunt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Emprunt>> GetReservationsForStudentAsync(string cin)
    {
        return await _context.Emprunts
            .Where(e => e.Cin == cin)
            .Include(e => e.NuminvNavigation)
            .OrderByDescending(e => e.Dateemprunt)
            .ThenByDescending(e => e.CreatedAt)
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
        var livre = await _context.Livres
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Numinventaire == numInventaire);

        if (livre is null)
        {
            throw new InvalidOperationException("Livre introuvable.");
        }

        var arrivalDate = livre.Datearrivage?.ToDateTime(TimeOnly.MinValue).Date;
        if (arrivalDate.HasValue && startDate.Date < arrivalDate.Value)
        {
            throw new InvalidOperationException("La date de debut ne peut pas etre anterieure a la date d'arrivage du livre.");
        }

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

    // Review/Rating methods
    public async Task<bool> CanReviewBookAsync(string cin, string numInventaire)
    {
        var hasReturnedReservation = await _context.Emprunts.AnyAsync(e =>
            e.Cin == cin &&
            e.Numinv == numInventaire &&
            e.Estretour.HasValue &&
            e.Estretour.Value >= e.Dateemprunt);

        return hasReturnedReservation;
    }

    public async Task CreateReviewAsync(string cin, string numInventaire, int note, string? commentaire)
    {
        if (note < 1 || note > 5)
        {
            throw new InvalidOperationException("La note doit etre entre 1 et 5.");
        }

        var canReview = await CanReviewBookAsync(cin, numInventaire);
        if (!canReview)
        {
            throw new InvalidOperationException("Vous ne pouvez pas evaluer ce livre. Vous devez d'abord l'emprunter et le retourner.");
        }

        var existingReview = await _context.Avis.FirstOrDefaultAsync(a =>
            a.Cin == cin && a.Numinv == numInventaire);
        if (existingReview is not null)
        {
            throw new InvalidOperationException("Vous avez deja evalue ce livre.");
        }

        var review = new Avis
        {
            Cin = cin,
            Numinv = numInventaire,
            Note = note,
            Commentaire = commentaire,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _context.Avis.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task<BookReviewsViewModel> GetReviewsForBookAsync(string numInventaire, string? currentUserCin = null)
    {
        var livre = await _context.Livres
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Numinventaire == numInventaire);

        if (livre is null)
        {
            return new BookReviewsViewModel
            {
                BookNumInv = numInventaire,
                BookTitle = "Livre introuvable",
                Reviews = new(),
                TotalReviews = 0,
                CanCurrentUserReview = false
            };
        }

        var reviews = await _context.Avis
            .Where(a => a.Numinv == numInventaire)
            .Include(a => a.Etudiant)
            .OrderByDescending(a => a.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        var reviewVMs = new List<AvisViewModel>();
        foreach (var a in reviews)
        {
            // Get return status: check if student has a returned reservation for this book
            var returnedReservation = await _context.Emprunts
                .Where(e => e.Cin == a.Cin && 
                            e.Numinv == numInventaire && 
                            e.Estretour.HasValue)
                .OrderByDescending(e => e.Estretour)
                .FirstOrDefaultAsync();

            var vm = new AvisViewModel
            {
                Id = a.Id,
                Note = a.Note,
                Commentaire = a.Commentaire,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                AuthorCin = a.Cin,
                AuthorName = GetAnonymousName(a.Etudiant?.Prenom, a.Etudiant?.Nom),
                BookReturned = returnedReservation != null,
                BookReturnDate = returnedReservation?.Estretour
            };
            reviewVMs.Add(vm);
        }

        var averageRating = reviews.Count > 0
            ? (decimal)reviews.Average(a => a.Note)
            : (decimal?)null;

        var currentUserReview = currentUserCin != null
            ? reviewVMs.FirstOrDefault(r => r.AuthorCin == currentUserCin)
            : null;

        var canCurrentUserReview = currentUserCin != null
            ? await CanReviewBookAsync(currentUserCin, numInventaire)
            : false;

        return new BookReviewsViewModel
        {
            BookNumInv = numInventaire,
            BookTitle = livre.Titre,
            AverageRating = averageRating,
            TotalReviews = reviews.Count,
            Reviews = reviewVMs,
            CanCurrentUserReview = canCurrentUserReview && currentUserReview is null,
            CurrentUserExistingReview = currentUserReview
        };
    }

    public async Task<AvisViewModel?> GetReviewByIdAsync(decimal reviewId)
    {
        var review = await _context.Avis
            .Include(a => a.Etudiant)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == reviewId);

        if (review is null)
        {
            return null;
        }

        return new AvisViewModel
        {
            Id = review.Id,
            Note = review.Note,
            Commentaire = review.Commentaire,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            AuthorCin = review.Cin,
            AuthorName = GetAnonymousName(review.Etudiant?.Prenom, review.Etudiant?.Nom)
        };
    }

    public async Task<bool> UpdateReviewAsync(decimal reviewId, string cin, int note, string? commentaire)
    {
        if (note < 1 || note > 5)
        {
            throw new InvalidOperationException("La note doit etre entre 1 et 5.");
        }

        var review = await _context.Avis.FirstOrDefaultAsync(a => a.Id == reviewId);
        if (review is null)
        {
            return false;
        }

        if (!string.Equals(review.Cin, cin, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        review.Note = note;
        review.Commentaire = commentaire;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReviewAsync(decimal reviewId, string cin)
    {
        var review = await _context.Avis.FirstOrDefaultAsync(a => a.Id == reviewId);
        if (review is null)
        {
            return false;
        }

        if (!string.Equals(review.Cin, cin, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        _context.Avis.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }

    private static string GetAnonymousName(string? prenom, string? nom)
    {
        if (string.IsNullOrWhiteSpace(prenom) && string.IsNullOrWhiteSpace(nom))
        {
            return "Anonyme";
        }

        var firstName = string.IsNullOrWhiteSpace(prenom) ? "A" : prenom.Substring(0, 1).ToUpper();
        var lastNameInitial = string.IsNullOrWhiteSpace(nom) ? "." : nom.Substring(0, 1).ToUpper() + ".";

        return $"{firstName} {lastNameInitial}";
    }
}