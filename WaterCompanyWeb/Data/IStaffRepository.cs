using System.Linq;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public interface IStaffRepository : IGenericRepository<Staff>
    {
        public IQueryable GetAllWithUsers();
    }
}
