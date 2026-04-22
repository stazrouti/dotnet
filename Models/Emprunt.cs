using System;
using System.Collections.Generic;

namespace Bibliotheque.Models;

public partial class Emprunt
{
    public int Id { get; set; }

    public string Cin { get; set; } = null!;

    public string Numinv { get; set; } = null!;

    public DateTime? Dateemprunt { get; set; }

    public DateTime? Dateretour { get; set; }

    public DateTime? Estretour { get; set; }

    public bool CanCancel { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }



    public bool IsLate =>
        Estretour != null &&
        Estretour < DateTime.Now;

    public virtual Etudiant CinNavigation { get; set; } = null!;

    public virtual Livre NuminvNavigation { get; set; } = null!;
}
