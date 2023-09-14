using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Client> Clients { get; set; }

        public DbSet<WaterMeter> WaterMeters { get; set; }

        public DbSet<Staff> Staff { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
