namespace Bibliotheque.Areas.Etudiant.DTOs;

public class AvisViewModel
{
    public decimal Id { get; set; }
    public int Note { get; set; } // 1-5
    public string? Commentaire { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? AuthorCin { get; set; }
    public string? AuthorName { get; set; } // First name + Last initial (privacy)
    public bool BookReturned { get; set; } // Whether the book was returned by reviewer
    public DateTime? BookReturnDate { get; set; } // When the book was returned
}

public class BookReviewsViewModel
{
    public string? BookNumInv { get; set; }
    public string? BookTitle { get; set; }
    public decimal? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<AvisViewModel> Reviews { get; set; } = new();
    public bool CanCurrentUserReview { get; set; }
    public AvisViewModel? CurrentUserExistingReview { get; set; }
}
