using Microsoft.EntityFrameworkCore;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
