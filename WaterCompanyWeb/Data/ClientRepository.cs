using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Data
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(DataContext context) : base(context)
        {

        }
    }
}
