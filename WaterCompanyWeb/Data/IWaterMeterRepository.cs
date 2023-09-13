using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public interface IWaterMeterRepository : IGenericRepository<WaterMeter>
    {
        IEnumerable<SelectListItem> GetComboClients();

        public Task<WaterMeter> GetWaterMeterWithClients(int id);

        public IQueryable GetAllWithClients();

        public Task DeleteWaterMeterAsync(int id);

        public Task<Client> GetClientsAsync(int id);

        public IQueryable GetAllByClient(int id);
    }
}
