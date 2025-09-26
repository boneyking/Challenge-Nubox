using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Configurations
{
    public class RegistroAsistenciaConfiguration : IEntityTypeConfiguration<RegistroAsistencia>
    {
        public void Configure(EntityTypeBuilder<RegistroAsistencia> builder)
        {
            builder.ToTable("RegistrosAsistencia");

            builder.Property(e => e.HorasRegulares)
                .HasPrecision(8, 2);

            builder.Property(e => e.HorasExtras)
                .HasPrecision(8, 2);

            builder.Property(e => e.Observaciones)
                .HasMaxLength(500);

            builder.Property(e => e.IdExternoPartner)
                .HasMaxLength(50);

            builder.Property(e => e.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(e => e.CreadoPor)
                .HasMaxLength(100);

            // Índices para consultas frecuentes
            builder.HasIndex(e => new { e.TrabajadorId, e.Fecha })
                .HasDatabaseName("IX_RegistrosAsistencia_TrabajadorFecha");

            builder.HasIndex(e => new { e.Fecha, e.Estado })
                .HasDatabaseName("IX_RegistrosAsistencia_FechaEstado");

            builder.HasIndex(e => e.IdExternoPartner)
                .HasDatabaseName("IX_RegistrosAsistencia_IdExternoPartner");

            // Relaciones
            builder.HasOne(e => e.Trabajador)
                .WithMany(t => t.RegistrosAsistencia)
                .HasForeignKey(e => e.TrabajadorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
