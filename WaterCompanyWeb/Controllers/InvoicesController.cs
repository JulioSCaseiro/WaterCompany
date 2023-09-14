using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterCompanyWeb.Data;
using WaterCompanyWeb.Data.Entities;
using WaterCompanyWeb.Helpers;

namespace WaterCompanyWeb.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public InvoicesController(IInvoiceRepository invoiceRepository,
            IWaterMeterRepository waterMeterRepository,
            IClientRepository clientRepository,
            IUserHelper userHelper,
            IMailHelper mailHelper)
        {
            _invoiceRepository = invoiceRepository;
            _waterMeterRepository = waterMeterRepository;
            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }

        // GET: Invoices
        public IActionResult Index()
        {
            var invoices = _invoiceRepository.GetAllInvoices();
            return View(invoices);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var invoices = await _invoiceRepository.GetByIdAsync(id.Value);
            if (invoices == null)
            {
                return NotFound();
            }
            var model = new Invoice();
            model.Date = invoices.Date;
            model.Client = invoices.Client;
            model.User = invoices.User;
            model.Value = invoices.Value;
            return View(model);

        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Invoice model)
        {
            if (ModelState.IsValid)
            {
                var invoices = await _invoiceRepository.GetByIdAsync(id.Value);
                if (invoices != null)
                {
                    invoices.Date = model.Date;
                    invoices.User = model.User;
                    invoices.Client = model.Client;
                    invoices.WaterMeter = model.WaterMeter;
                    invoices.Value = invoices.Value;


                    await _invoiceRepository.UpdateAsync(invoices);
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("InvoiceNotFound");
            }

            var invoice = await _invoiceRepository.GetByIdAsync(id.Value);
            if (invoice == null)
            {
                return new NotFoundViewResult("InvoiceNotFound");
            }
            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var invoice = await _invoiceRepository.GetByIdAsync(id);
            await _invoiceRepository.DeleteAsync(invoice);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult InvoiceNotFound()
        {
            return View();
        }
    }
}
