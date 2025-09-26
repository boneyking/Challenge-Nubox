using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Configurations
{
    public class TransaccionSincronizacionConfiguration : IEntityTypeConfiguration<TransaccionSincronizacion>
    {
        public void Configure(EntityTypeBuilder<TransaccionSincronizacion> builder)
        {
            builder.ToTable("TransaccionesSincronizacion");

            builder.Property(e => e.TransactionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.DetalleRequest)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.DetalleResponse)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.MensajeError)
                .HasMaxLength(2000);

            builder.Property(e => e.ProcesadoPor)
                .HasMaxLength(100);

            builder.Property(e => e.TamañoArchivoMB)
                .HasPrecision(10, 2);

            // Índices
            builder.HasIndex(e => e.TransactionId)
                .IsUnique()
                .HasDatabaseName("IX_TransaccionesSincronizacion_TransactionId");

            builder.HasIndex(e => new { e.CompaniaId, e.FechaInicio })
                .HasDatabaseName("IX_TransaccionesSincronizacion_CompaniaFecha");

            builder.HasIndex(e => new { e.Estado, e.FechaInicio })
                .HasDatabaseName("IX_TransaccionesSincronizacion_EstadoFecha");

            // Relaciones
            builder.HasOne(e => e.Compania)
                .WithMany()
                .HasForeignKey(e => e.CompaniaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
