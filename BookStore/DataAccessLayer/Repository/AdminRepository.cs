using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
   public class AdminRepository:IAdminRepository
    {


        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Admin> _passwordHasher;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = new PasswordHasher<Admin>();


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
    }



}

