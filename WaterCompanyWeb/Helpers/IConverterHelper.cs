using System;
using WaterCompanyWeb.Data.Entities;
using WaterCompanyWeb.Models;

namespace WaterCompanyWeb.Helpers
{
    public interface IConverterHelper
    {
        Client ToClient(ClientViewModel model, string path, bool isNew);

        ClientViewModel ToClientViewModel(Client client);

        Staff ToStaff(StaffViewModel model, string path, bool isNew);

        StaffViewModel ToStaffViewModel(Staff employee);
    }
}
