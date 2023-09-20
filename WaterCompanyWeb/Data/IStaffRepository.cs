using System.Linq;
using System.Threading.Tasks;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public interface IStaffRepository : IGenericRepository<Staff>
    {
        public IQueryable GetAllWithUsers();

        Task<Staff> GetStaffByEmailAsync(string username);
    }
}
