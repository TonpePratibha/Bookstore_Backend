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
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _config;


        public UserRepository(ApplicationDbContext context, JwtHelper jwtHelper,IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = new PasswordHasher<User>();
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    


        public UserModel RegisterUser(UserModel userModel)
        {
            try
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

                return new UserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error registering user: {ex.Message}");
            }
        }

        public LoginResponse ValidateUser(UserLogin userLoginModel)
        {
            try
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

                var token = _jwtHelper.GenerateToken(user.Email, user.Role, user.Id);

                return new LoginResponse
                {
                    Token = token,
                    Email = user.Email,
                    FirstName = user.FirstName
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error validating user: {ex.Message}");
            }
        }

        public UserModel getUserById(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return null;

                return new UserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user by id: {ex.Message}");
            }
        }

        public void DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return;
                }

                _context.Remove(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}");
            }
        }

        public void UpdateUser(int id, UserModel model)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all users: {ex.Message}");
            }
        }

        public void SendResetPasswordEmail(string email)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                string token = _jwtHelper.GenerateResetToken(user.Id, "user");
                string resetLink = $"http://localhost:4200/reset/{token}";

                SendEmail(user.Email, "Password Reset", $"Click here to reset your password: {resetLink}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending reset password email: {ex.Message}");
            }
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = _config["EmailSettings:FromEmail"];
                var password = _config["EmailSettings:Password"];
                var smtpServer = _config["EmailSettings:SmtpServer"];
                var port = int.Parse(_config["EmailSettings:Port"]);

                using (var smtpClient = new SmtpClient(smtpServer, port))
                using (var mailMessage = new MailMessage(fromEmail, toEmail, subject, body))
                {
                    smtpClient.Credentials = new NetworkCredential(fromEmail, password);
                    smtpClient.EnableSsl = true;

                    mailMessage.IsBodyHtml = true;
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending email: {ex.Message}");
            }
        }





        public string ResetPassword(string token, string newPassword)
            {
                int userId;
                try
                {
                    userId = _jwtHelper.ExtractUserIdFromJwt(token);
                }
                catch (Exception ex)
                {
                    
                    throw new Exception("Invalid token");
                }

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    
                    throw new Exception("User not found");
                }

                user.Password = _passwordHasher.HashPassword(user, newPassword);
                _context.SaveChanges();

               
                return "Password reset successful";
            }

      


        public RefreshLoginResponse AcesstokenLogin(UserLogin userLoginModel)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == userLoginModel.Email);
                if (user == null) return null;

                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userLoginModel.Password);
                if (result != PasswordVerificationResult.Success) return null;

                var accessToken = _jwtHelper.GenerateToken(user.Email, user.Role, user.Id);
                var refreshToken = Guid.NewGuid().ToString();

                var tokenEntry = new RolebasedRefreshToken
                {
                    EntityId = user.Id,
                    Role = user.Role,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
                    RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
                };

                _context.RoleBasedRefreshTokens.Add(tokenEntry);
                _context.SaveChanges();

                return new RefreshLoginResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Email = user.Email,
                    FirstName = user.FirstName
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during access token login: {ex.Message}");
            }
        }

        public RefreshLoginResponse RefreshAccessToken(string refreshToken)
        {
            try
            {
                var token = _context.RoleBasedRefreshTokens.FirstOrDefault(t =>
                    t.RefreshToken == refreshToken && t.RefreshTokenExpiry > DateTime.UtcNow);

                if (token == null) return null;

                var user = _context.Users.FirstOrDefault(u => u.Id == token.EntityId);
                if (user == null) return null;

                var newAccessToken = _jwtHelper.GenerateToken(token.Role, user.Email, user.Id);
                token.AccessToken = newAccessToken;
                token.AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15);
                _context.SaveChanges();

                return new RefreshLoginResponse
                {
                    Token = newAccessToken,
                    RefreshToken = token.RefreshToken,
                    Email = user.Email,
                    FirstName = user.FirstName
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error refreshing access token: {ex.Message}");
            }
        }


    }



}







    



