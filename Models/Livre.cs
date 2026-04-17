using System;
using System.Collections.Generic;

namespace Bibliotheque.Models;

public partial class Livre
{
    public string Numinventaire { get; set; } = null!;

    public string? Isbn { get; set; }

    public string? Titre { get; set; }

    public string? Auteur { get; set; }

    public string? Editeur { get; set; }

    public DateOnly? Datepublication { get; set; }

    public DateOnly? Datearrivage { get; set; }

    public string? Résumé { get; set; }

    public string? Cote { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();

    public virtual ICollection<Motcle> Motcles { get; set; } = new List<Motcle>();
}
