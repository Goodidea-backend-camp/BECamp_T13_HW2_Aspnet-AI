using BECamp_T13_HW2_Aspnet_AI.Models;
using Microsoft.EntityFrameworkCore;

namespace BECamp_T13_HW2_Aspnet_AI.Data
{
    public class RegisterContext : DbContext
    {
        public RegisterContext(DbContextOptions<RegisterContext> registerOptions)
            : base(registerOptions)
        {
        }

        public DbSet<Register> Registers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder registerBuilder)
        {
            registerBuilder.Entity<Register>().ToTable("Account");
        }
    }
}