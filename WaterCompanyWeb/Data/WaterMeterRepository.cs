using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public class WaterMeterRepository : GenericRepository<WaterMeter>, IWaterMeterRepository
    {
        private readonly DataContext _context;

        public WaterMeterRepository(DataContext context) : base(context)
        {
            _context = context;
        }


        public async Task DeleteWaterMeterAsync(int id)
        {
            var waterMeter = await _context.WaterMeters.FindAsync(id);
            if (waterMeter == null)
            {
                return;
            }

            _context.WaterMeters.Remove(waterMeter);
            await _context.SaveChangesAsync();
        }

        public IQueryable GetAllByClient(string email)
        {
            return _context.WaterMeters.Where(c => c.Client.Email == email);
        }

        public IQueryable GetAllWithClients()
        {
            return _context.WaterMeters.Include(p => p.Client).OrderBy(p => p.Id);
        }

        public async Task<Client> GetClientsAsync(int id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public IEnumerable<SelectListItem> GetComboClients()
        {
            var list = _context.Clients.Select(c => new SelectListItem
            {
                Text = c.FirstName + " " + c.LastName,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Select a client!",
                Value = "0"
            });

            return list;
        }

        public async Task<WaterMeter> GetWaterMeterWithClients(int id)
        {
            return await _context.WaterMeters
                .Include(p => p.Client)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
