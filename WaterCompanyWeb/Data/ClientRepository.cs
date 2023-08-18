using Microsoft.EntityFrameworkCore;
using System.Linq;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly DataContext _context;

        public ClientRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Clients.Include(p => p.User);
        }
    }
}
