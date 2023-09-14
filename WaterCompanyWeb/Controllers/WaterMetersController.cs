using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WaterCompanyWeb.Data;
using WaterCompanyWeb.Data.Entities;
using WaterCompanyWeb.Helpers;
using WaterCompanyWeb.Models;

namespace WaterCompanyWeb.Controllers
{
    public class WaterMetersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;

        public WaterMetersController(
            IUserHelper userHelper,
            IClientRepository clientRepository,
            IWaterMeterRepository waterMeterRepository)
        {
            _userHelper = userHelper;
            _clientRepository = clientRepository;
            _waterMeterRepository = waterMeterRepository;
        }

        // GET: WaterMeters
        [RoleAuthorization("Staff")]
        public IActionResult Index()
        {
            var waterMeters = _waterMeterRepository.GetAllWithClients();
            return View(waterMeters);
        }

        // GET: WaterMeters/Details/5
        [RoleAuthorization("Staff", "Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }

            var waterMeter = await _waterMeterRepository.GetWaterMeterWithClients(id.Value);
            if (waterMeter == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }

            return View(waterMeter);
        }

        // GET: WaterMeters/Create
        [RoleAuthorization("Staff")]
        public IActionResult Create()
        {
            var model = new WaterMeterViewModel
            {
                Clients = _waterMeterRepository.GetComboClients(),
            };

            return View(model);
        }

        // POST: WaterMeters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization("Staff")]
        public async Task<IActionResult> Create(WaterMeterViewModel model)
        {
            var client = await _waterMeterRepository.GetClientsAsync(model.ClientId);
            if (ModelState.IsValid)
            {

                double value = 0;
                double totalConsumption = model.TotalConsumption;

                do
                {
                    if (totalConsumption > 25)
                    {
                        do
                        {
                            value += 1.60;
                            totalConsumption -= 1;
                        } while (totalConsumption > 25);
                    }
                    else
                    {
                        if (totalConsumption > 15)
                        {
                            do
                            {
                                value += 1.20;
                                totalConsumption -= 1;
                            } while (totalConsumption > 15);
                        }
                        else if (totalConsumption > 5)
                        {
                            do
                            {
                                value += 0.80;
                                totalConsumption -= 1;
                            } while (totalConsumption > 5);
                        }
                        else if (totalConsumption > 0)
                        {
                            do
                            {
                                value += 0.30;
                                totalConsumption -= 1;
                            } while (totalConsumption > 0);
                        }

                    }
                } while (totalConsumption > 0);

                WaterMeter waterMeter = new WaterMeter
                {
                    Client = client,
                    ConsumptionDate = model.ConsumptionDate,
                    Value = Math.Round(value, 2),
                    TotalConsumption = Math.Round(model.TotalConsumption, 2)
                };

                await _waterMeterRepository.CreateAsync(waterMeter);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: WaterMeters/Create
        [RoleAuthorization("Client")]
        public async Task<IActionResult> CreateByClient()
        {
            Client client = await _clientRepository.GetClientByEmailAsync(this.User.Identity.Name);
            if (client == null)
            {
                return RedirectToAction("ClientNotFound", "Clients");
            }
            var model = new WaterMeterViewModel
            {
                Client = client,
                ClientId = client.Id,
                Clients = _waterMeterRepository.GetComboClients()
            };

            return View(model);
        }

        // POST: WaterMeters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization("Client")]
        public async Task<IActionResult> CreateByClient(WaterMeterViewModel model)
        {
            Client client = await _clientRepository.GetClientByEmailAsync(this.User.Identity.Name);

            if (ModelState.IsValid)
            {

                double value = 0;
                double totalConsumption = model.TotalConsumption;

                do
                {
                    if (totalConsumption > 25)
                    {
                        do
                        {
                            value += 1.60;
                            totalConsumption -= 1;
                        } while (totalConsumption > 25);
                    }
                    else
                    {
                        if (totalConsumption > 15)
                        {
                            do
                            {
                                value += 1.20;
                                totalConsumption -= 1;
                            } while (totalConsumption > 15);
                        }
                        else if (totalConsumption > 5)
                        {
                            do
                            {
                                value += 0.80;
                                totalConsumption -= 1;
                            } while (totalConsumption > 5);
                        }
                        else if (totalConsumption > 0)
                        {
                            do
                            {
                                value += 0.30;
                                totalConsumption -= 1;
                            } while (totalConsumption > 0);
                        }

                    }
                } while (totalConsumption > 0);

                WaterMeter waterMeter = new WaterMeter
                {
                    Client = client,
                    ConsumptionDate = model.ConsumptionDate,
                    Value = Math.Round(value, 2),
                    TotalConsumption = Math.Round(model.TotalConsumption, 2)
                };

                await _waterMeterRepository.CreateAsync(waterMeter);
                return RedirectToAction(nameof(WaterMetersByClient));
            }
            return View(model);
        }

        // GET: WaterMeters/Edit/5
        [RoleAuthorization("Staff")]
        public async Task<IActionResult> Edit(int? id)
        {
            var waterMeters = await _waterMeterRepository.GetWaterMeterWithClients(id.Value);
            var model = new WaterMeterViewModel();
            if (waterMeters == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }
            else
            {
                model.ClientId = model.Id;
                model.Value = waterMeters.Value;
                model.ConsumptionDate = waterMeters.ConsumptionDate;
                model.TotalConsumption = waterMeters.TotalConsumption;
            }
            model.Clients = _waterMeterRepository.GetComboClients();
            return View(model);
        }

        // POST: WaterMeters/Edit/5
        [HttpPost]
        [RoleAuthorization("Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WaterMeterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var waterMeter = await _waterMeterRepository.GetWaterMeterWithClients(id);
                if (waterMeter != null)
                {
                    var client = await _waterMeterRepository.GetClientsAsync(model.ClientId);
                    double value = 0;
                    double totalConsumption = model.TotalConsumption;

                    do
                    {
                        if (totalConsumption > 25)
                        {
                            do
                            {
                                value += 1.60;
                                totalConsumption -= 1;
                            } while (totalConsumption > 25);
                        }
                        else
                        {
                            if (totalConsumption > 15)
                            {
                                do
                                {
                                    value += 1.20;
                                    totalConsumption -= 1;
                                } while (totalConsumption > 15);
                            }
                            else if (totalConsumption > 5)
                            {
                                do
                                {
                                    value += 0.80;
                                    totalConsumption -= 1;
                                } while (totalConsumption > 5);
                            }
                            else if (totalConsumption > 0)
                            {
                                do
                                {
                                    value += 0.30;
                                    totalConsumption -= 1;
                                } while (totalConsumption > 0);
                            }

                        }
                    } while (totalConsumption > 0);

                    waterMeter.Client = client;
                    waterMeter.ConsumptionDate = model.ConsumptionDate;
                    waterMeter.Value = Math.Round(value);
                    waterMeter.TotalConsumption = model.TotalConsumption;

                    await _waterMeterRepository.UpdateAsync(waterMeter);
                    var allConsumptions = _waterMeterRepository.GetAllWithClients();
                    return View("Index", allConsumptions);
                }
            }
            return View(model);
        }

        // GET: WaterMeters/Delete/5
        [RoleAuthorization("Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }

            var waterMeter = await _waterMeterRepository.GetWaterMeterWithClients(id.Value);
            if (waterMeter == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }

            return View(waterMeter);
        }

        // POST: WaterMeters/Delete/5
        [RoleAuthorization("Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waterMeter = await _waterMeterRepository.GetByIdAsync(id);
            await _waterMeterRepository.DeleteAsync(waterMeter);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> WaterMetersByClient()
        {
            User user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }
            return View(_waterMeterRepository.GetAllByClient(user.Email));
        }
        public IActionResult WaterMeterNotFound()
        {
            return View();
        }
        
    }
}
