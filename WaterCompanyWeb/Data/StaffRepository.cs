using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
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


        public async Task<Staff> GetStaffByEmailAsync(string email)
        {
            return await _context.Staff.Where(c => c.Email == email).FirstOrDefaultAsync();
        }
    }
}
