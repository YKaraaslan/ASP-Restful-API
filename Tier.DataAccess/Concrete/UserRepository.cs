using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tier.DataAccess.Abstract;
using Tier.Entities;
using Tier.Shared;
using Tier.DataAccess;

namespace Tier.DataAccess.Concrete
{
    public class UserRepository : IUserRepository
    {
        private UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<User>> GetUsers()
        {
            using (var dbContext = new TierDbContext())
            {
                return await dbContext.Users.ToListAsync();
            }
        }

        public async Task<UserModel> GetUserByUserName(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                return new UserModel
                {
                    NameSurname = user.Name + " " + user.Surname,
                    userID = user.Id,
                    Department = user.Department,
                    Position = user.Position
                };
            }

            return new UserModel();
        }

        public async Task<User> UpdateUser(User user)
        {
            using (var dbContext = new TierDbContext())
            {
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync();
                return user;
            }
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterModel model)
        {
            if (model == null)
                throw new NullReferenceException("Register model is null");

            if (model.Password != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Confirm password does not match the password",
                    IsSuccess = false
                };
            }

            var identityUser = new User
            {
                Email = model.Username,
                UserName = model.Username,
                Department = model.Department,
                Name = model.Name,
                Surname = model.Surname,
                Position = model.Position
            };

            model.Role = "User";

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(identityUser, model.Role);
                return new UserManagerResponse
                {
                    Message = "User created successfully",
                    IsSuccess = true
                };
            }

            return new UserManagerResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Username);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "There is no user with that Email address",
                    IsSuccess = false
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid Password",
                    IsSuccess = false
                };
            }

            var role = await _userManager.GetRolesAsync(user);
            IdentityOptions _options = new IdentityOptions();

            var claims = new[]
            {
                new Claim("Username", model.Username),
                new Claim(ClaimTypes.NameIdentifier, model.Username),
                new Claim(_options.ClaimsIdentity.RoleClaimType, role.FirstOrDefault())
            };

            var keyBuffer = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("W#P=V[nL+Khe_MS{c<.Qj7,2xu46~vJ#PWmHVB=t2!$`gUe8hBX"));

            var token = new JwtSecurityToken(
                issuer: "http://yk.com",
                audience: "http://yk.com",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(keyBuffer, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpirationDate = token.ValidTo
            };
        }

        public async Task<UserManagerResponse> UserExists(string id)
        {
            if (await _userManager.FindByIdAsync(id) != null)
            {
                return new UserManagerResponse
                {
                    Message = "User exists in the database",
                    IsSuccess = true
                };
            }
            return new UserManagerResponse
            {
                Message = "There is no user with the ID",
                IsSuccess = false
            };
        }

        private static string calculateTime(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("{0} {1}", years, "yıl");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("{0} {1}", months, "ay");
            }
            if (span.Days > 0)
                return String.Format("{0} {1}", span.Days, "gün");
            if (span.Hours > 0)
                return String.Format("{0} {1}", span.Hours, "saat");
            if (span.Minutes > 5)
                return String.Format("{0} {1}", span.Minutes, "dakika");
            else
                return "online";
            return string.Empty;
        }
    }
}
