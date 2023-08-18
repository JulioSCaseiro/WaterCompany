using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using WaterCompanyWeb.Data;
using WaterCompanyWeb.Data.Entities;
using WaterCompanyWeb.Helpers;
using WaterCompanyWeb.Models;

namespace WaterCompanyWeb.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;

        public ClientsController(IClientRepository clientRepository,
            IUserHelper userHelper)
        {
            _clientRepository = clientRepository;
            _userHelper = userHelper;
        }

        // GET: Clients
        public IActionResult Index()
        {
            return View(_clientRepository.GetAll().OrderBy(p => p.ClientName));
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\clients",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/clients/{file}";
                }
                var client = this.ToClient(model, path);

                client.User = await _userHelper.GetUserByEmailAsync("caseiroinc@gmail.com");
                await _clientRepository.CreateAsync(client);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private Client ToClient(ClientViewModel model, string path)
        {
            return new Client
            {
                Id = model.Id,
                ClientName = model.ClientName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                PostalCode = model.PostalCode,
                NIF = model.NIF,
                ImageUrl = path,
                IsAvailable = model.IsAvailable,
                User = model.User
            };
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return NotFound();
            }
            var model = this.ToClientViewModel(client);
            return View(model);

        }

        private ClientViewModel ToClientViewModel(Client client)
        {
            return new ClientViewModel
            {
                Id = client.Id,
                ClientName = client.ClientName,
                Phone = client.Phone,
                Email = client.Email,
                Address = client.Address,
                PostalCode = client.PostalCode,
                NIF = client.NIF,
                ImageUrl = client.ImageUrl,
                IsAvailable = client.IsAvailable,
                User = client.User
            };
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        var file = $"{guid}.png";

                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\clients",
                            file);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/clients/{file}";
                    }

                    var client = this.ToClient(model, path);

                    //TODO: Modificar para o user que tiver logado
                    client.User = await _userHelper.GetUserByEmailAsync("caseiroinc@gmail.com");
                    await _clientRepository.UpdateAsync(client);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _clientRepository.ExistAsync(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            await _clientRepository.DeleteAsync(client);
            return RedirectToAction(nameof(Index));
        }
    }
}
