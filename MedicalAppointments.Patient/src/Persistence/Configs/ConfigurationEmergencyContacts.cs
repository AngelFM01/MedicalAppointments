using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs
{
    public class ConfigurationEmergencyContacts : IEntityTypeConfiguration<EmergencyContacts>
    {
        public void Configure(EntityTypeBuilder<EmergencyContacts> builder)
        {
            //Configuracion de llave primaria
            builder.HasKey(x => x.IdContactEmerg);

            //configuracion de llaveforanea
            builder.HasAlternateKey(x => x.PacientID);
        }
    }
}
