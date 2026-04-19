using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Bibliotheque.Models;

public partial class User : IdentityUser
{
     public string NomComplet  { get; set; } = string.Empty;

     public string Role { get; set; } = string.Empty;
}
