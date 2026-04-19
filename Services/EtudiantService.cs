using Bibliotheque.Models;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheque.Services;

public class EtudiantService : IEtudiantService
{
    private readonly BibliothequeContext _context;

    public EtudiantService(BibliothequeContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Etudiant>> GetAllAsync(string? searchTerm = null)
    {
        var query = _context.Etudiants.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(e =>
                (e.Cin != null && e.Cin.ToLower().Contains(term)) ||
                (e.Matricule != null && e.Matricule.ToLower().Contains(term)) ||
                (e.Nom != null && e.Nom.ToLower().Contains(term)) ||
                (e.Prenom != null && e.Prenom.ToLower().Contains(term)) ||
                (e.Email != null && e.Email.ToLower().Contains(term)));
        }

        return await query
            .OrderBy(e => e.Nom)
            .ThenBy(e => e.Prenom)
            .ToListAsync();
    }

    public async Task<Etudiant?> GetByIdAsync(string cin)
    {
        return await _context.Etudiants
            .Include(e => e.Emprunts)
            .Include(e => e.Visites)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Cin == cin);
    }

    public async Task<Etudiant?> GetByMatriculeAsync(string matricule)
    {
        return await _context.Etudiants
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Matricule == matricule);
    }

    public async Task<bool> ExistsAsync(string cin)
    {
        return await _context.Etudiants.AnyAsync(e => e.Cin == cin);
    }

    public async Task<bool> MatriculeExistsAsync(string matricule, string? excludeCin = null)
    {
        var query = _context.Etudiants.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(excludeCin))
        {
            query = query.Where(e => e.Cin != excludeCin);
        }

        return await query.AnyAsync(e => e.Matricule == matricule);
    }

    public async Task<bool> EmailExistsAsync(string email, string? excludeCin = null)
    {
        var query = _context.Etudiants.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(excludeCin))
        {
            query = query.Where(e => e.Cin != excludeCin);
        }

        return await query.AnyAsync(e => e.Email == email);
    }

    public async Task<bool> CreateAsync(Etudiant etudiant)
    {
        if (await ExistsAsync(etudiant.Cin))
        {
            return false;
        }

        // Validate unique constraints
        if (!string.IsNullOrWhiteSpace(etudiant.Matricule) && await MatriculeExistsAsync(etudiant.Matricule))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(etudiant.Email) && await EmailExistsAsync(etudiant.Email))
        {
            return false;
        }

        etudiant.CreatedAt = DateTime.UtcNow;
        etudiant.UpdatedAt = DateTime.UtcNow;

        _context.Etudiants.Add(etudiant);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(Etudiant etudiant)
    {
        var existing = await _context.Etudiants.FirstOrDefaultAsync(e => e.Cin == etudiant.Cin);
        if (existing is null)
        {
            return false;
        }

        // Validate unique Matricule
        if (!string.IsNullOrWhiteSpace(etudiant.Matricule) && 
            etudiant.Matricule != existing.Matricule && 
            await MatriculeExistsAsync(etudiant.Matricule, etudiant.Cin))
        {
            return false;
        }

        // Validate unique Email
        if (!string.IsNullOrWhiteSpace(etudiant.Email) && 
            etudiant.Email != existing.Email && 
            await EmailExistsAsync(etudiant.Email, etudiant.Cin))
        {
            return false;
        }

        existing.Matricule = etudiant.Matricule;
        existing.Nom = etudiant.Nom;
        existing.Prenom = etudiant.Prenom;
        existing.Email = etudiant.Email;
        existing.Niveau = etudiant.Niveau;
        existing.Filiere = etudiant.Filiere;
        existing.Tel = etudiant.Tel;
        existing.Rfid = etudiant.Rfid;
        existing.Qr = etudiant.Qr;
        existing.Commentaire = etudiant.Commentaire;
        existing.Observation = etudiant.Observation;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string cin)
    {
        var etudiant = await _context.Etudiants.FirstOrDefaultAsync(e => e.Cin == cin);
        if (etudiant is null)
        {
            return false;
        }

        // Check for related loans or visits
        var hasLoans = await _context.Emprunts.AnyAsync(e => e.Cin == cin);
        var hasVisits = await _context.Visites.AnyAsync(v => v.Cin == cin);

        if (hasLoans || hasVisits)
        {
            // Don't delete if student has associated records
            return false;
        }

        _context.Etudiants.Remove(etudiant);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Emprunt>> GetActiveLoansAsync(string cin)
    {
        return await _context.Emprunts
            .Where(e => e.Cin == cin && !e.Dateretour.HasValue)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Visite>> GetRecentVisitesAsync(string cin, int days = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return await _context.Visites
            .Where(v => v.Cin == cin && v.CreatedAt >= cutoffDate)
            .OrderByDescending(v => v.UpdatedAt)
            .AsNoTracking()
            .ToListAsync();
    }
}
