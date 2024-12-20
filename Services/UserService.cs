using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using System.Numerics;
using TimeSync.Core.model;
using TimeSync.Infrastructure;
using TimeSync.Core.DTO.BusinessDTO;
using Microsoft.EntityFrameworkCore;

namespace TimeSync.Services
{
    public class UserService
    {
        private readonly UserContext _userContext;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(UserContext userContext)
        {
            _userContext = userContext;
            _passwordHasher = new PasswordHasher<User>();

        }

        private User MapToUser(UserDTO userDTO)
        {
            return new User
            {
                UserId = userDTO.UserId,
                Password = userDTO.Password,
                Email = userDTO.Email,

            };
        }

        private UserDTO MapToUserDTO(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                Password = user.Password,
                Email = user.Email,

            };
        }

        public async Task<UserDTO?> CreateUserAsync(UserDTO userDTO)
        {
            if (await _userContext.Users.AnyAsync(u => u.Email == userDTO.Email))
            {
                return null; // User already exists
            }

            var user = MapToUser(userDTO);
            user.Password = _passwordHasher.HashPassword(user, userDTO.Password);

            _userContext.Users.Add(user);
            await _userContext.SaveChangesAsync();



            return MapToUserDTO(user);
        }



        public async Task<UserDTO?> LoginAsync(string Email, string userPassword)
        {
            var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userPassword);

            if (result != PasswordVerificationResult.Success)
            {
                return null; // Invalid password
            }

            await _userContext.SaveChangesAsync();

            return MapToUserDTO(user);
        }


    }
}
