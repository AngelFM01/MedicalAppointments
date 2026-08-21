using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public sealed class ContactoEmergenciaConfiguration : IEntityTypeConfiguration<ContactoEmergencia>
{
    public void Configure(EntityTypeBuilder<ContactoEmergencia> builder)
    {
        builder.ToTable("EmergencyContacts", "dbo");
        builder.HasKey(x => x.ContactoEmergenciaId);
        builder.Property(x => x.ContactoEmergenciaId).HasColumnName("ContactoEmergenciaID");
        builder.Property(x => x.PacienteId).HasColumnName("PacienteID");
        builder.Property(x => x.NombreCompleto).HasColumnName("NombreCompleto").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Parentesco).HasColumnName("Parentesco").HasMaxLength(100);
        builder.Property(x => x.Telefono).HasColumnName("Telefono").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(x => x.TelefonoSecundario).HasColumnName("TelefonoSecundario").HasMaxLength(30).IsUnicode(false);
        builder.Property(x => x.Email).HasColumnName("Email").HasMaxLength(150).IsUnicode(false);
        builder.Property(x => x.Prioridad).HasColumnName("Prioridad").HasDefaultValue(1);
        builder.Property(x => x.Activo).HasColumnName("Activo").HasDefaultValue(true);
        builder.HasOne(x => x.Paciente).WithMany(x => x.ContactosEmergencia).HasForeignKey(x => x.PacienteId).OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(x => x.PacienteId).HasDatabaseName("IX_EmergencyContacts_Patient");
    }
}
