using System;
using System.Collections.Generic;

namespace Bibliotheque.Models;

public partial class Etudiant
{
    public string Cin { get; set; } = null!;

    public string? Matricule { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? Email { get; set; }

    public string? Niveau { get; set; }

    public string? Filiere { get; set; }

    public string? Tel { get; set; }

    public string? Rfid { get; set; }

    public string? Qr { get; set; }

    public string? Commentaire { get; set; }

    public string? Observation { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();

    public virtual ICollection<Visite> Visites { get; set; } = new List<Visite>();

    public virtual ICollection<Avis> Avis { get; set; } = new List<Avis>();
}
