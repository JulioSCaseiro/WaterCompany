using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {

        public IQueryable GetAllWithClients();

        public Task<Client> GetClientsAsync(int id);

        public IQueryable GetAllByClient(string email);

        public IQueryable GetAllInvoices();


    }
}
