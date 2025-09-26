using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth
{
    public class AuthContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public AuthContext()
        {
        }
        public AuthContext(DbContextOptions<AuthContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("AuthConnection"));
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
