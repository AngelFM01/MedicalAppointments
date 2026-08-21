using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public sealed class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Patients", "dbo");
        builder.HasKey(x => x.PacienteId);
        builder.Property(x => x.PacienteId).HasColumnName("PacienteID");
        builder.Property(x => x.CodigoPaciente).HasColumnName("CodigoPaciente").HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.HasIndex(x => x.CodigoPaciente).IsUnique();
        builder.Property(x => x.TipoDocumento).HasColumnName("TipoDocumento").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(x => x.NumeroDocumento).HasColumnName("NumeroDocumento").HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.HasIndex(x => new { x.TipoDocumento, x.NumeroDocumento }).IsUnique();
        builder.Property(x => x.Nombres).HasColumnName("Nombres").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Apellidos).HasColumnName("Apellidos").HasMaxLength(100).IsRequired();
        builder.Property(x => x.FechaNacimiento).HasColumnName("FechaNacimiento").HasColumnType("date");
        builder.Property(x => x.Sexo).HasColumnName("Sexo").HasMaxLength(1).IsUnicode(false).IsFixedLength().IsRequired();
        builder.Property(x => x.EstadoCivil).HasColumnName("EstadoCivil").HasMaxLength(30).IsUnicode(false);
        builder.Property(x => x.Telefono).HasColumnName("Telefono").HasMaxLength(30).IsUnicode(false);
        builder.Property(x => x.TelefonoSecundario).HasColumnName("TelefonoSecundario").HasMaxLength(30).IsUnicode(false);
        builder.Property(x => x.Email).HasColumnName("Email").HasMaxLength(150).IsUnicode(false);
        builder.Property(x => x.Direccion).HasColumnName("Direccion").HasMaxLength(300);
        builder.Property(x => x.Ciudad).HasColumnName("Ciudad").HasMaxLength(100);
        builder.Property(x => x.Pais).HasColumnName("Pais").HasMaxLength(100);
        builder.Property(x => x.Ocupacion).HasColumnName("Ocupacion").HasMaxLength(150);
        builder.Property(x => x.TipoSangre).HasColumnName("TipoSangre").HasMaxLength(10).IsUnicode(false);
        builder.Property(x => x.Activo).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(x => x.FechaRegistro).HasColumnName("FechaRegistro").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
    }
}
