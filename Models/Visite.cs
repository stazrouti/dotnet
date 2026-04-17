using System;
using System.Collections.Generic;

namespace Bibliotheque.Models;

public partial class Visite
{
    public long Id { get; set; }

    public string? Cin { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Etudiant? CinNavigation { get; set; }
}
