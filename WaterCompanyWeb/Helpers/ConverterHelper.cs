using System;
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
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Address = model.Address,
                ZIPCode = model.ZIPCode,
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
                FirstName = client.FirstName,
                LastName = client.LastName,
                PhoneNumber = client.PhoneNumber,
                Email = client.Email,
                Address = client.Address,
                ZIPCode = client.ZIPCode,
                NIF = client.NIF,
                ImageUrl = client.ImageUrl,
                IsAvailable = client.IsAvailable,
                User = client.User
            };
        }

        public Staff ToStaff(StaffViewModel model, string path, bool isNew)
        {
            return new Staff
            {
                Id = isNew ? 0 : model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                ZIPCode = model.ZIPCode,
                NIF = model.NIF,
                Email = model.Email,
                ImageUrl = path,
                IsAvailable = model.IsAvailable,
                User = model.User
                
            };
        }

        public StaffViewModel ToStaffViewModel(Staff staff)
        {
            return new StaffViewModel
            {
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                PhoneNumber = staff.PhoneNumber,
                Address = staff.Address,
                ZIPCode = staff.ZIPCode,
                NIF = staff.NIF,
                Email = staff.Email,
                ImageUrl = staff.ImageUrl,
                IsAvailable = staff.IsAvailable,
                User = staff.User
            };
        }

        public User ToUser(ChangeUserViewModel model, string path, bool isNew)
        {
            return new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Address = model.Address,
                ZIPCode = model.ZIPCode,
                NIF = model.NIF,
                ImageUrl = path
            };
        }

        public ChangeUserViewModel ToUserViewModel(User user)
        {
            return new ChangeUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                ZIPCode = user.ZIPCode,
                NIF = user.NIF,
                ImageUrl = user.ImageUrl
            };
        }
    }
}
