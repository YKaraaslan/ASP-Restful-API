using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tier.Business.Abstract;
using Tier.DataAccess.Abstract;
using Tier.Entities;
using Tier.Shared;

namespace Tier.Business.Concrete
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _userRepository.GetUsers();
        }

        public async Task<UserModel> GetUserByUserName(string userName)
        {
            return await _userRepository.GetUserByUserName(userName);
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterModel model)
        {
            return await _userRepository.RegisterUserAsync(model);
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginModel model)
        {
            return await _userRepository.LoginUserAsync(model);
        }

        public async Task<UserManagerResponse> UserExists(string id)
        {
            return await _userRepository.UserExists(id);
        }
    }
}
