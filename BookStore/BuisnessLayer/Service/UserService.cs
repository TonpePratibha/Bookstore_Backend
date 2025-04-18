using BuisnessLayer.Interface;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Service
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;


        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;

        }

        public void RegisterUser(UserModel userModel)
        {
            _userRepository.RegisterUser(userModel);

        }

        public string ValidateUser(UserLogin userLoginModel)
        {

            var token = _userRepository.ValidateUser(userLoginModel);

            if (token == null)
            {
                return "token s nulll";
            }
            return token;

           



        }

        public UserModel getUserById(int id)
        {
            return _userRepository.getUserById(id);

        }

        public void Deleteuser(int id) { 
           _userRepository.DeleteUser(id);

        }

        public List<UserModel> GetAllUsers()
        {
          
                var users = _userRepository.GetAllUsers();

                return users.Select(u => new UserModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }).ToList();
            }


        public void UpdateUser(int id, UserModel model)
        {
            _userRepository.UpdateUser(id, model);
        }

        public void SendResetPasswordEmail(string email)
        {
            _userRepository.SendResetPasswordEmail(email);
        }
        public string ResetPassword(string token, string newPassword)
        {
            return _userRepository.ResetPassword(token, newPassword);
        }


    }
}



