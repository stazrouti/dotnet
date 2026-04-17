using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bibliotheque.Models
{
    public class BibliothequeContextFactory : IDesignTimeDbContextFactory<BibliothequeContext>
    {
        public BibliothequeContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BibliothequeContext>();

            // optionsBuilder.UseSqlServer("Data Source=.\\SQLExpress;Initial Catalog=bibliotheque;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=bibliotheque;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

            return new BibliothequeContext(optionsBuilder.Options);
        }
    }
}
