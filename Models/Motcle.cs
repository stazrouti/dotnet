using System;
using System.Collections.Generic;

namespace Bibliotheque.Models;

public partial class Motcle
{
    public decimal Id { get; set; }

    public string Numinv { get; set; } = null!;

    public string Mot { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Livre NuminvNavigation { get; set; } = null!;
}
