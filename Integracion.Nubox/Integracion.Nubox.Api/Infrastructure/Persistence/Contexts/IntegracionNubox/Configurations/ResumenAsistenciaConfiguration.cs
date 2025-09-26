using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Configurations
{
    public class ResumenAsistenciaConfiguration : IEntityTypeConfiguration<ResumenAsistencia>
    {
        public void Configure(EntityTypeBuilder<ResumenAsistencia> builder)
        {
            builder.ToTable("ResumenesAsistencia");

            builder.Property(e => e.TotalHorasRegulares)
                .HasPrecision(8, 2);

            builder.Property(e => e.TotalHorasExtras)
                .HasPrecision(8, 2);

            // Índice único por trabajador y periodo
            builder.HasIndex(e => new { e.TrabajadorId, e.Año, e.Mes, e.Semana, e.Quincena })
                .IsUnique()
                .HasDatabaseName("IX_ResumenesAsistencia_TrabajadorPeriodo");

            // Índices para consultas de liquidaciones
            builder.HasIndex(e => new { e.EsProcesado, e.Año, e.Mes })
                .HasDatabaseName("IX_ResumenesAsistencia_ProcesadoPeriodo");

            // Relaciones
            builder.HasOne(e => e.Trabajador)
                .WithMany()
                .HasForeignKey(e => e.TrabajadorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
