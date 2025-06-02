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
        private readonly IRabitmqProducer _rabbitmqProducer;

        private readonly ILogger<UserRepository> _logger;
        public UserRepository(ApplicationDbContext context, JwtHelper jwtHelper,IConfiguration configuration, IRabitmqProducer rabbitmqProducer, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = new PasswordHasher<User>();
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _rabbitmqProducer=rabbitmqProducer ?? throw new ArgumentNullException(nameof(rabbitmqProducer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    


        public UserModel RegisterUser(UserModel userModel)
        {
            try
            {
                _logger.LogInformation("Registering user: {Email}", userModel.Email);
                if (UserExists(userModel.Email))
                {
                    _logger.LogWarning("User already exists with email: {Email}", userModel.Email);
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
                _logger.LogInformation("User registered successfully: {Email}", user.Email);
                return new UserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Email}", userModel.Email);
                throw new Exception($"Error registering user: {ex.Message}");
            }
        }

        public LoginResponse ValidateUser(UserLogin userLoginModel)
        {
            try
            {
                _logger.LogInformation("Validating user: {Email}", userLoginModel.Email);

                var user = _context.Users.FirstOrDefault(u => u.Email == userLoginModel.Email);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Email}", userLoginModel.Email);
                    return null;
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userLoginModel.Password);
                if (result != PasswordVerificationResult.Success)
                {
                    _logger.LogWarning("Invalid password for user: {Email}", userLoginModel.Email);
                    return null;
                }

                var token = _jwtHelper.GenerateToken(user.Email, user.Role, user.Id);
                _logger.LogInformation("User validated successfully: {Email}", user.Email);
                return new LoginResponse
                {
                    Token = token,
                    Email = user.Email,
                    FirstName = user.FirstName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user: {Email}", userLoginModel.Email);
                throw new Exception($"Error validating user: {ex.Message}");
            }
        }

        public UserModel GetUserById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching user by ID: {Id}", id);
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
                _logger.LogError(ex, "Error fetching user by ID: {Id}", id);
                throw new Exception($"Error fetching user by id: {ex.Message}");
            }
        }

        public void DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user with ID: {Id}", id);
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return;
                }

                _context.Remove(user);
                _context.SaveChanges();
                _logger.LogInformation("User deleted successfully: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Id}", id);
                throw new Exception($"Error deleting user: {ex.Message}");
            }
        }

        public void UpdateUser(int id, UserModel model)
        {
            try
            {
                _logger.LogInformation("Updating user with ID: {Id}", id);
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning("User not found for update: {Id}", id);
                    throw new InvalidOperationException("User not found.");
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                _context.SaveChanges();
                _logger.LogInformation("User updated successfully: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Id}", id);
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                throw new Exception($"Error fetching all users: {ex.Message}");
            }
        }
       




        public void SendResetPasswordEmail(string email)   
        {  
            try
            {
                _logger.LogInformation("Sending reset password email to: {Email}", email);
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    _logger.LogWarning("User not found for password reset: {Email}", email);
                    throw new InvalidOperationException("User not found.");
                }

                string token = _jwtHelper.GenerateResetToken(user.Id, "user");
                string resetLink = $"http://localhost:4200/reset/{token}";

                var emailMessage = new EmailMessage
                {
                    ToEmail = user.Email,
                    Subject = "Password Reset",
                    Body = $"Click here to reset your password: {resetLink}"
                };

                _rabbitmqProducer.PublishEmailMessage(emailMessage); // msg send through queue //producer

                _logger.LogInformation("Reset password email queued for: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error queuing reset password email: {Email}", email);
                throw new Exception($"Error queuing reset password email: {ex.Message}");
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
                _logger.LogInformation("Email sent successfully to: {ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to: {ToEmail}", toEmail);
                throw new Exception($"Error sending email: {ex.Message}");
            }
        }





        public string ResetPassword(string token, string newPassword)
            {
                int userId;
                try
                {
                _logger.LogInformation("Resetting password using token");
                userId = _jwtHelper.ExtractUserIdFromJwt(token);
                }
                catch (Exception ex)
                {
                    
                    throw new Exception("Invalid token");
                }

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                _logger.LogWarning("User not found for password reset");
                throw new Exception("User not found");
                }

                user.Password = _passwordHasher.HashPassword(user, newPassword);
                _context.SaveChanges();
            _logger.LogInformation("Password reset successful for user: {Id}", user.Id);

            return "Password reset successful";
            }

      


        public RefreshLoginResponse AcesstokenLogin(UserLogin userLoginModel)
        {
            try
            {
                _logger.LogInformation("Access token login attempt for: {Email}", userLoginModel.Email);
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

                _logger.LogInformation("Access token login successful for: {Email}", user.Email);


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
                _logger.LogError(ex, "Error during access token login: {Email}", userLoginModel.Email);
                throw new Exception($"Error during access token login: {ex.Message}");
            }
        }

        public RefreshLoginResponse RefreshAccessToken(string refreshToken)
        {
            try
            {
                _logger.LogInformation("Refreshing access token for refresh token: {RefreshToken}", refreshToken);
                var token = _context.RoleBasedRefreshTokens.FirstOrDefault(t =>
                    t.RefreshToken == refreshToken && t.RefreshTokenExpiry > DateTime.UtcNow);

                if (token == null) return null;

                var user = _context.Users.FirstOrDefault(u => u.Id == token.EntityId);
                if (user == null) return null;

                var newAccessToken = _jwtHelper.GenerateToken(token.Role, user.Email, user.Id);
                token.AccessToken = newAccessToken;
                token.AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15);
                _context.SaveChanges();
                _logger.LogInformation("Access token refreshed successfully for user: {Id}", user.Id);

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
                _logger.LogError(ex, "Error refreshing access token for token: {RefreshToken}", refreshToken);
                throw new Exception($"Error refreshing access token: {ex.Message}");
            }
        }


    }



}







    



