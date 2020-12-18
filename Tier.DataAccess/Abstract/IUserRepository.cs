using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tier.Entities;
using Tier.Shared;

namespace Tier.DataAccess.Abstract
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsers();
        Task<UserModel> GetUserByUserName(string userName);
        Task<UserManagerResponse> RegisterUserAsync(RegisterModel model);
        Task<UserManagerResponse> LoginUserAsync(LoginModel model);
        Task<UserManagerResponse> UserExists(string id);
    }
}
