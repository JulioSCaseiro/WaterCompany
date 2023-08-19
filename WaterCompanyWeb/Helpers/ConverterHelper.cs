using System.IO;
using WaterCompanyWeb.Data.Entities;
using WaterCompanyWeb.Models;

namespace WaterCompanyWeb.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Client ToClient(ClientViewModel model, string path, bool isNew)
        {
            return new Client
            {
                Id = isNew ? 0 : model.Id,
                ClientName = model.ClientName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                PostalCode = model.PostalCode,
                NIF = model.NIF,
                ImageUrl = path,
                IsAvailable = model.IsAvailable,
                User = model.User
            };
        }

        public ClientViewModel ToClientViewModel(Client client)
        {
            return new ClientViewModel
            {
                Id = client.Id,
                ClientName = client.ClientName,
                Phone = client.Phone,
                Email = client.Email,
                Address = client.Address,
                PostalCode = client.PostalCode,
                NIF = client.NIF,
                ImageUrl = client.ImageUrl,
                IsAvailable = client.IsAvailable,
                User = client.User
            };
        }
    }
}
