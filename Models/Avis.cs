namespace Bibliotheque.Models;

public class Avis
{
    public decimal Id { get; set; }
    public string? Cin { get; set; }
    public string? Numinv { get; set; }
    public int Note { get; set; } // 1-5
    public string? Commentaire { get; set; } // Optional review text
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Etudiant? Etudiant { get; set; }
    public virtual Livre? Livre { get; set; }
}
