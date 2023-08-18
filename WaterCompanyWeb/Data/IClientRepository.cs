using System.Linq;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        public IQueryable GetAllWithUsers();
    }
}
