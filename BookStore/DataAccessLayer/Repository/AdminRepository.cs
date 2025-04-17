using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.JWT;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
   public class AdminRepository:IAdminRepository
    {


        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Admin> _passwordHasher;
        private readonly JwtHelper _jwtHelper;
        public AdminRepository(ApplicationDbContext context,JwtHelper jwtHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = new PasswordHasher<Admin>();
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
        }

        public bool AdminExists(string email)
        {
            return _context.Admin.Any(u => u.Email == email);
        }

        public void RegisterAdmin(AdminModel adminModel)
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


        }


        public string ValidateAdmin( AdminLogin adminLoginModel) {

            var admin = _context.Admin.FirstOrDefault(u => u.Email==adminLoginModel.Email);
            if (admin == null) {
                return null;
            }
            var result = _passwordHasher.VerifyHashedPassword(admin, admin.Password, adminLoginModel.Password);

            if (result != PasswordVerificationResult.Success) {
                return null;

            }

            return _jwtHelper.GenerateToken(admin.Email, admin.Role, admin.Id);
        }



    }



}

