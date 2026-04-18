using GivingChampion.Application.Interfaces.User;
using GivingChampion.Common.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Services
{
    public class UserService : IUserService
    {
        public Task CreateAdminUserByAdmin(CreateUserDto dto)
        {
            throw new NotImplementedException();
        }

        public Task CreateUserByAdmin(CreateUserDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUser(DeleteUser dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetUserDto>> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public Task<GetUserDto> GetUserById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUser(UpdateUser dto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserByAdmin(Guid id, UpdateUser dto)
        {
            throw new NotImplementedException();
        }
    }
}
