using DataAccessLayer.Entity;
using DataAccessLayer.Modal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataContext
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Book> Books { get; set; }

        public DbSet<RolebasedRefreshToken> RoleBasedRefreshTokens { get; set; }

        public DbSet<Cart> Cart{ get; set; }

        public DbSet<WishList> Wishlist { get; set; }
        public DbSet<CustomerDetails> customerDetails { get; set; }
        public DbSet<OrderDetails> orderDetails { get; set; }
        public DbSet<Feedback> feedbacks { get; set; }
        public DbSet<WishlListItemModel> WishlistBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            modelBuilder.Entity<WishlListItemModel>().HasNoKey();
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RolebasedRefreshToken>().ToTable("RoleBasedRefreshTokens");


            modelBuilder.Entity<Cart>()
        .Property(c => c.Price)
        .HasPrecision(18, 2);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.PurchasedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Carts)
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<WishList>()
              .HasOne(w => w.User)
              .WithMany(u => u.Wishlist)
              .HasForeignKey(w => w.AddedBy)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishList>()
                .HasOne(w => w.Book)
                .WithMany(b => b.Wishlist)
                .HasForeignKey(w => w.BookId)
                .OnDelete(DeleteBehavior.Cascade);


         
        }

    }
}
