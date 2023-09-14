using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly DataContext _context;
        public InvoiceRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        public IQueryable GetAllByClient(string email)
        {
            return _context.Invoices.Where(c => c.Client.Email == email);
        }

        public IQueryable GetAllInvoices()
        {
            return _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.WaterMeter);
        }

        public IQueryable GetAllWithClients()
        {
            return _context.Invoices.Include(p => p.Client);
        }

        public async Task<Client> GetClientsAsync(int id)
        {
            return await _context.Clients.FindAsync(id);
        }
    }
}
