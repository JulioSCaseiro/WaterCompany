using Microsoft.EntityFrameworkCore;
using System.Linq;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        private readonly DataContext _context;

        public StaffRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Staff.Include(p => p.User).OrderBy(p => p.Id);
        }
    }
}
