using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class PatientConfigType : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.DocumentNumber).HasColumnName("document_number").HasMaxLength(25).IsRequired();
        builder.HasIndex(x => x.DocumentNumber).IsUnique();
        builder.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.BirthDate).HasColumnName("birth_date").HasColumnType("date").IsRequired();
        builder.Property(x => x.Sex).HasColumnName("sex").HasConversion<string>().HasMaxLength(10).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(254).IsRequired();
        builder.Property(x => x.Address).HasColumnName("address").HasMaxLength(300).IsRequired();
        builder.Property(x => x.MaritalStatus).HasColumnName("marital_status").HasConversion<string>().HasMaxLength(10).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
    }
}
