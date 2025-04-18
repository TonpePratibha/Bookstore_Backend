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
                Email = adminModel.Email,

            };
        }


        public LoginResponse ValidateAdmin(AdminLogin adminLoginModel) {

            var admin = _context.Admin.FirstOrDefault(u => u.Email == adminLoginModel.Email);
            if (admin == null) {
                return null;
            }
            var result = _passwordHasher.VerifyHashedPassword(admin, admin.Password, adminLoginModel.Password);

            if (result != PasswordVerificationResult.Success) {
                return null;

            }

            var token= _jwtHelper.GenerateToken(admin.Email, admin.Role, admin.Id);

            return new LoginResponse
            {

                Token = token,
                Email = admin.Email,
                FirstName = admin.FirstName

            };
        }


        public AdminModel getAdinById(int id) {

            var admin = _context.Admin.FirstOrDefault(u => u.Id == id);


            if (admin == null) {
                return null;
            }






            return new AdminModel
            {
                //Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email

            };

        }


        public void UpdateAdmin(int id, AdminModel model)
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

        public void DeleteAdmin(int id)
        {
            var admin = _context.Admin.FirstOrDefault(u => u.Id == id);
            if (admin == null)
            {
                return;

            }

            _context.Remove(admin);
            _context.SaveChanges();



        }

        
               public void SendResetPasswordEmail(string email)
                {
                    var admin = _context.Admin.FirstOrDefault(u => u.Email == email);
                    if (admin == null)
                    {

                        throw new InvalidOperationException("User not found.");
                    }

                    string token = _jwtHelper.GenerateResetToken(admin.Id ,"admin");
                    

        string resetLink = $"http://localhost:4200/reset/{token}";


        SendEmail(admin.Email, "Password Reset", $"Click here to reset your password: {resetLink}");

    }

    public void SendEmail(string toEmail, string subject, string body)
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

}  
}





