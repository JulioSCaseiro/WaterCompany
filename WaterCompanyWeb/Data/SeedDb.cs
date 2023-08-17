using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Clients.Any())
            {
                AddClient("Jorge Manuel");
                AddClient("Pedro Teixeira");
                AddClient("Eduardo Fonseca");
                await _context.SaveChangesAsync();
            }
        }

        private void AddClient(string name)
        {
            _context.Clients.Add(new Client
            {
                ClientName = name,
                Phone = Convert.ToString(_random.Next(930000000, 969999999)),
                Address = "Rua dos testes",
                PostalCode = "6969-404",
                NIF = Convert.ToString(_random.Next(000000000, 999999999)),
                Email = "esteemailetop@gmail.com",
                IsAvailable = true
            });
        }
    }
}
