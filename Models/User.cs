using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Bibliotheque.Models;

public partial class User : IdentityUser
{
     public string NomComplet  { get; set; } = null;

    public string Role { get; set; } = null;

    public string CIN { get; set; } = null;

    public int Niveau { get; set; }

    public string Filiere { get; set; } = null;
}
