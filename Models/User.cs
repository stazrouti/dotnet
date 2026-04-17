using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Bibliotheque.Models;

public partial class User : IdentityUser
{
     public string NomComplet  { get; set; }

     public string Role { get; set; }
}
