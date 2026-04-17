using System;
using System.Collections.Generic;

namespace Bibliotheque.Models;

public partial class Emprunt
{
    public decimal Id { get; set; }

    public string Cin { get; set; } = null!;

    public string Numinv { get; set; } = null!;

    public DateTime? Dateemprunt { get; set; }

    public DateTime? Dateretour { get; set; }

    public DateTime? Estretour { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Etudiant CinNavigation { get; set; } = null!;

    public virtual Livre NuminvNavigation { get; set; } = null!;
}
