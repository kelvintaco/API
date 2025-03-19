using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace APIFinal.Context
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args = null)
        {
            var options = new DbContextOptionsBuilder<DataContext>();
            options.UseSqlServer("Server=.\\SQLEXPRESS;Database=SystemDB;Trusted_Connection=True;TrustServerCertificate=True");

            return new DataContext(options.Options);
        }

    }
}
