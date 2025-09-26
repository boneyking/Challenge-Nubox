using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Configurations
{
    public class LicenciaMedicaConfiguration : IEntityTypeConfiguration<LicenciaMedica>
    {
        public void Configure(EntityTypeBuilder<LicenciaMedica> builder)
        {
            builder.ToTable("LicenciasMedicas");

            builder.Property(e => e.TipoLicencia)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.NumeroLicencia)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Diagnostico)
                .HasMaxLength(500);

            builder.Property(e => e.IdExternoPartner)
                .HasMaxLength(50);

            // Índices
            builder.HasIndex(e => e.NumeroLicencia)
                .IsUnique()
                .HasDatabaseName("IX_LicenciasMedicas_NumeroLicencia");

            builder.HasIndex(e => new { e.TrabajadorId, e.FechaInicio, e.FechaFin })
                .HasDatabaseName("IX_LicenciasMedicas_TrabajadorPeriodo");

            // Relaciones
            builder.HasOne(e => e.Trabajador)
                .WithMany(t => t.LicenciasMedicas)
                .HasForeignKey(e => e.TrabajadorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
