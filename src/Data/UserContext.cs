using BECamp_T13_HW2_Aspnet_AI.Models;
using Microsoft.EntityFrameworkCore;

namespace BECamp_T13_HW2_Aspnet_AI.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set;}
    }
}