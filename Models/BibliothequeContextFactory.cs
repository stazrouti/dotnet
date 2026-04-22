using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bibliotheque.Models
{
    public class BibliothequeContextFactory : IDesignTimeDbContextFactory<BibliothequeContext>
    {
        public BibliothequeContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BibliothequeContext>();

            var builder = WebApplication.CreateBuilder(args);

            // optionsBuilder.UseSqlServer("Data Source=.\\SQLExpress;Initial Catalog=bibliotheque;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

            optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            return new BibliothequeContext(optionsBuilder.Options);
        }
    }
}
