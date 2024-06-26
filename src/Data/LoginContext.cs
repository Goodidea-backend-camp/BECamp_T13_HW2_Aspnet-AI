using BECamp_T13_HW2_Aspnet_AI.Models;
using Microsoft.EntityFrameworkCore;

namespace BECamp_T13_HW2_Aspnet_AI.Data
{
    public class LoginContext : DbContext
    {
        public LoginContext(DbContextOptions<LoginContext> loginOptions)
            : base(loginOptions)
        {
        }

        public DbSet<Login> Logins { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder loginBuilder)
        {
            loginBuilder.Entity<Login>().ToTable("Account");
        }
    }
}