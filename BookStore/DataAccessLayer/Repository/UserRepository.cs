using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtHelper _jwtHelper;


        public UserRepository(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = new PasswordHasher<User>();
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public void RegisterUser(UserModel userModel)
        {
            if (UserExists(userModel.Email))
            {
                throw new Exception("User already exists with this email.");
            }

            var user = new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                Role = "user"

            };

            user.Password = _passwordHasher.HashPassword(user, userModel.Password);
            _context.Users.Add(user);
            _context.SaveChanges();


        }

        public string ValidateUser(UserLogin userLoginModel)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == userLoginModel.Email);
            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userLoginModel.Password);
            if (result != PasswordVerificationResult.Success)
            {

                return null;
            }


            return _jwtHelper.GenerateToken(user.Email, user.Role, user.Id);
        }


        public UserModel getUserById(int id) {

            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            return new UserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,


            };


           
            
            

        

        }

    }
}
