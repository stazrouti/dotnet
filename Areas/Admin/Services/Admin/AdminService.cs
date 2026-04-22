using Bibliotheque.Models;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheque.Areas.Admin.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly BibliothequeContext _context;

        public AdminService(BibliothequeContext context)
        {
            _context = context;
        }

        public async Task<Boolean> UpdateEtudiant(string? searchTerm = null)
        {

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                Bibliotheque.Models.Etudiant etudiant = await _context.Etudiants.Where(et => et.Email == term).FirstOrDefaultAsync();
                if ((etudiant!=null))
                {
                    _context.Etudiants.Remove(etudiant);
                await _context.SaveChangesAsync();
                return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<Boolean> AddEtudiant(User? utilisateur = null)
        {

            if (utilisateur != null)
            {
                Bibliotheque.Models.Etudiant etud = await _context.Etudiants.Where(_ => _.Cin == utilisateur.CIN).FirstOrDefaultAsync();

                if (etud == null)
                {
                    _context.Etudiants.Add(new Models.Etudiant
                    {
                        Cin = utilisateur.CIN,
                        Email = utilisateur.Email,
                        Nom = utilisateur.NomComplet,
                        Filiere = utilisateur.Filiere,
                        Niveau = utilisateur.Niveau,
                        Matricule = utilisateur.CIN,
                        Qr = utilisateur.CIN,
                        Rfid = utilisateur.CIN
                    });
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
