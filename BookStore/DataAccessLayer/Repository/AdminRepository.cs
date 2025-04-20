using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class AdminRepository : IAdminRepository
    {


        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Admin> _passwordHasher;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _config;
        public AdminRepository(ApplicationDbContext context, JwtHelper jwtHelper, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = new PasswordHasher<Admin>();
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public bool AdminExists(string email)
        {
            return _context.Admin.Any(u => u.Email == email);
        }

       

        public AdminModel RegisterAdmin(AdminModel adminModel)
        {
            try
            {
                if (AdminExists(adminModel.Email))
                {
                    throw new Exception("User already exists with this email.");
                }

                var admin = new Admin
                {
                    FirstName = adminModel.FirstName,
                    LastName = adminModel.LastName,
                    Email = adminModel.Email,
                    Role = "Admin"
                };

                admin.Password = _passwordHasher.HashPassword(admin, adminModel.Password);
                _context.Admin.Add(admin);
                _context.SaveChanges();

                return new AdminModel
                {
                    FirstName = adminModel.FirstName,
                    LastName = adminModel.LastName,
                    Email = adminModel.Email
                };
            }
            catch (Exception ex)
            {
                
                throw new Exception("An error occurred while registering the admin.", ex);
            }
        }


        

        public LoginResponse ValidateAdmin(AdminLogin adminLoginModel)
        {
            try
            {
                var admin = _context.Admin.FirstOrDefault(u => u.Email == adminLoginModel.Email);
                if (admin == null)
                {
                    return null;
                }

                var result = _passwordHasher.VerifyHashedPassword(admin, admin.Password, adminLoginModel.Password);

                if (result != PasswordVerificationResult.Success)
                {
                    return null;
                }

                var token = _jwtHelper.GenerateToken(admin.Email, admin.Role, admin.Id);

                return new LoginResponse
                {
                    Token = token,
                    Email = admin.Email,
                    FirstName = admin.FirstName
                };
            }
            catch (Exception ex)
            {
                
                return null; 
            }
        }
      

        public AdminModel GetAdminById(int id)
        {
            try
            {
                var admin = _context.Admin.FirstOrDefault(u => u.Id == id);
                if (admin == null) return null;

                return new AdminModel
                {
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Email = admin.Email
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving admin.", ex);
            }
        }

        public void UpdateAdmin(int id, AdminModel model)
        {
            try
            {
                var user = _context.Admin.FirstOrDefault(u => u.Id == id);
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
                throw new Exception("Error updating admin.", ex);
            }
        }

        public void DeleteAdmin(int id)
        {
            try
            {
                var admin = _context.Admin.FirstOrDefault(u => u.Id == id);
                if (admin == null)
                {
                    return;
                }

                _context.Remove(admin);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting admin.", ex);
            }
        }

        public void SendResetPasswordEmail(string email)
        {
            try
            {
                var admin = _context.Admin.FirstOrDefault(u => u.Email == email);
                if (admin == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                string token = _jwtHelper.GenerateResetToken(admin.Id, "admin");
                string resetLink = $"http://localhost:4200/reset/{token}";

                SendEmail(admin.Email, "Password Reset", $"Click here to reset your password: {resetLink}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending reset password email.", ex);
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
                throw new Exception("Error sending email.", ex);
            }
        }




        public string ResetPassword(string token, string newPassword)
    {
        int adminId;
        try
        {
            adminId = _jwtHelper.ExtractUserIdFromJwt(token);
        }
        catch (Exception ex)
        {

            throw new Exception("Invalid token");
        }

        var user = _context.Admin.FirstOrDefault(u => u.Id == adminId);
        if (user == null)
        {

            throw new Exception("User not found");
        }

        user.Password = _passwordHasher.HashPassword(user, newPassword);
        _context.SaveChanges();


        return "Password reset successful";
    }

       

        public RefreshLoginResponse AccesstokenLogin(AdminLogin adminLoginModel)
        {
            try
            {
                var admin = _context.Admin.FirstOrDefault(u => u.Email == adminLoginModel.Email);
                if (admin == null) return null;

                var result = _passwordHasher.VerifyHashedPassword(admin, admin.Password, adminLoginModel.Password);
                if (result != PasswordVerificationResult.Success) return null;

               
                var accessToken = _jwtHelper.GenerateToken(admin.Email, admin.Role, admin.Id);
                var refreshToken = Guid.NewGuid().ToString();

                
                var tokenEntry = new RolebasedRefreshToken
                {
                    EntityId = admin.Id,
                    Role = admin.Role,
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
                    Email = admin.Email,
                    FirstName = admin.FirstName
                };
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        public RefreshLoginResponse RefreshAccessToken(string refreshToken)
        {
            try
            {
                var token = _context.RoleBasedRefreshTokens.FirstOrDefault(t =>
                    t.RefreshToken == refreshToken && t.RefreshTokenExpiry > DateTime.UtcNow);

                if (token == null) return null;

                var admin = _context.Admin.FirstOrDefault(u => u.Id == token.EntityId);
                if (admin == null) return null;

                var newAccessToken = _jwtHelper.GenerateToken(token.Role, admin.Email, admin.Id);
                token.AccessToken = newAccessToken;
                token.AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15);
                _context.SaveChanges();

                return new RefreshLoginResponse
                {
                    Token = newAccessToken,
                    RefreshToken = token.RefreshToken,
                    Email = admin.Email,
                    FirstName = admin.FirstName
                };
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Token refresh error: {ex.Message}");
                return null;
            }
        }





    }
}





