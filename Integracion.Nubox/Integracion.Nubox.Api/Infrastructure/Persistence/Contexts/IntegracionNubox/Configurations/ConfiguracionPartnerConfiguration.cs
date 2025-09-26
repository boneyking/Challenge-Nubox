using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Configurations
{
    public class ConfiguracionPartnerConfiguration : IEntityTypeConfiguration<ConfiguracionPartner>
    {
        public void Configure(EntityTypeBuilder<ConfiguracionPartner> builder)
        {
            builder.ToTable("ConfiguracionesPartner");

            builder.Property(e => e.NombrePartner)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.BaseUrl)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.ApiKey)
                .HasMaxLength(255);

            builder.Property(e => e.ClientId)
                .HasMaxLength(100);

            builder.Property(e => e.ClientSecret)
                .HasMaxLength(255);

            builder.Property(e => e.EmailNotificaciones)
                .HasMaxLength(255);

            // Índices
            builder.HasIndex(e => e.CompaniaId)
                .IsUnique()
                .HasDatabaseName("IX_ConfiguracionesPartner_CompaniaId");

            // Relaciones
            builder.HasOne(e => e.Compania)
                .WithMany()
                .HasForeignKey(e => e.CompaniaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
