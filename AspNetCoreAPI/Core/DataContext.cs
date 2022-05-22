using Microsoft.EntityFrameworkCore;
using ASPNetCoreAPI.Entities;
using Microsoft.Extensions.Configuration;

namespace ASPNetCoreAPI.Core
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
        
        public DbSet<UserEntity> UserEntities { get; set; }
    }
}