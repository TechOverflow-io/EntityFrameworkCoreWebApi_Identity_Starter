using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Model;

namespace WebApplication5.Data
{
    public class MyDbContext : IdentityDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {   
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .Property(u => u.Height)
                .IsRequired();

            builder.Entity<User>()
                .Property(u => u.Nickname)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
