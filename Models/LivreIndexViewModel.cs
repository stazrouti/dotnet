namespace Bibliotheque.Models;

public class LivreIndexViewModel
{
    public string? Search { get; set; }
    public List<Livre> Livres { get; set; } = new();
}