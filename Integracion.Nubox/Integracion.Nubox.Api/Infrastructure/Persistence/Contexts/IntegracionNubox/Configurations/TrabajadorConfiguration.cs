using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Configurations
{
    public class TrabajadorConfiguration : IEntityTypeConfiguration<Trabajador>
    {
        public void Configure(EntityTypeBuilder<Trabajador> builder)
        {
            builder.ToTable("Trabajadores");

            builder.Property(e => e.Nombres)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Apellidos)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Dni)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Email)
                .HasMaxLength(255);

            builder.Property(e => e.IdExternoPartner)
                .HasMaxLength(50);

            // Índices
            builder.HasIndex(e => new { e.CompaniaId, e.Dni })
                .IsUnique()
                .HasDatabaseName("IX_Trabajadores_CompaniaDni");

            builder.HasIndex(e => e.IdExternoPartner)
                .HasDatabaseName("IX_Trabajadores_IdExternoPartner");

            // Relaciones
            builder.HasOne(e => e.Compania)
                .WithMany()
                .HasForeignKey(e => e.CompaniaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
