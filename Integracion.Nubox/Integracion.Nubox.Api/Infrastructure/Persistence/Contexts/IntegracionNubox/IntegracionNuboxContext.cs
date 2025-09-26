using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox
{
    public class IntegracionNuboxContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public IntegracionNuboxContext()
        {
            
        }

        public IntegracionNuboxContext(DbContextOptions<IntegracionNuboxContext> options, 
            IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Compania> Companias => Set<Compania>();
        public DbSet<BitacoraSincronizacion> BitacorasSincronizacion => Set<BitacoraSincronizacion>();
        public DbSet<Trabajador> Trabajadores => Set<Trabajador>();
        public DbSet<RegistroAsistencia> RegistrosAsistencia => Set<RegistroAsistencia>();
        public DbSet<LicenciaMedica> LicenciasMedicas => Set<LicenciaMedica>();
        public DbSet<TransaccionSincronizacion> TransaccionesSincronizacion => Set<TransaccionSincronizacion>();
        public DbSet<ConfiguracionPartner> ConfiguracionesPartner => Set<ConfiguracionPartner>();
        public DbSet<ResumenAsistencia> ResumenesAsistencia => Set<ResumenAsistencia>();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("IntegracionNubox"));
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

    }
}
