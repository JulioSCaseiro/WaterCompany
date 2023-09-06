using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WaterCompanyWeb.Data.Entities;
using WaterCompanyWeb.Helpers;

namespace WaterCompanyWeb.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Staff");
            await _userHelper.CheckRoleAsync("Client");

            var user = await _userHelper.GetUserByEmailAsync("caseiroinc@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Júlio",
                    LastName = "Caseiro",
                    Email = "caseiroinc@gmail.com",
                    UserName = "caseiroinc@gmail.com",
                    PhoneNumber = "911789217"
                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Couldn't create user in seedDB!");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }


            if (!_context.Clients.Any())
            {
                AddClient("Jorge Manuel", user);
                AddClient("Pedro Teixeira", user);
                AddClient("Eduardo Fonseca", user);
                await _context.SaveChangesAsync();
            }
        }

        private void AddClient(string name, User user)
        {
            _context.Clients.Add(new Client
            {
                ClientName = name,
                Phone = Convert.ToString(_random.Next(930000000, 969999999)),
                Address = "Rua dos testes",
                PostalCode = "6969-404",
                NIF = Convert.ToString(_random.Next(000000000, 999999999)),
                Email = "esteemailetop@gmail.com",
                IsAvailable = true,
                User = user
            }) ;
        }
    }
}
