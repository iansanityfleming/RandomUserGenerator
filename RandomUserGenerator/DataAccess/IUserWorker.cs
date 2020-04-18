using RandomUserGenerator.DataAccess.DAO;
using RandomUserGenerator.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IUserWorker
    {
        Task<UserModel> GetUser(int id);
        Task<IEnumerable<UserModel>> GetUsers(IEnumerable<int> ids);
        Task<UserModel> UpdateUser(UserModel updatedUser);

        Task<bool> DeleteUser(int id);
    }
}
