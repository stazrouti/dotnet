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
