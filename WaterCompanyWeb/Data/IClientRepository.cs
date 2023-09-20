using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        public IQueryable GetAllWithUsers();

        Task<Client> GetClientByEmailAsync(string username);
    }
}
