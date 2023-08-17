using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WaterCompanyWeb.Data.Entities;

namespace WaterCompanyWeb.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);
    }
}