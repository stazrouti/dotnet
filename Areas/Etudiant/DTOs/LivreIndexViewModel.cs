using Bibliotheque.Models;

namespace Bibliotheque.Areas.Etudiant.DTOs;

public class LivreIndexViewModel
{
    public string? Search { get; set; }
    public List<Livre> Livres { get; set; } = new();

    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public int TotalPages { get; set; } = 1;

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}