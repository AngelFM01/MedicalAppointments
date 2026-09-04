using System.ComponentModel.DataAnnotations.Schema;
using Domain.Abstractions;

namespace Domain.Model;

public class Paciente : IEntity<long>
{
    public long PacienteId { get; set; }

    /// <summary>Alias genérico de la clave primaria exigido por <see cref="IEntity{TKey}"/>.
    /// No se mapea a columna: el repositorio genérico resuelve la clave real por metadatos de EF Core.</summary>
    [NotMapped]
    public long Id => PacienteId;
    public string CodigoPaciente { get; set; } = null!;
    public string TipoDocumento { get; set; } = null!;
    public string NumeroDocumento { get; set; } = null!;
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public DateOnly FechaNacimiento { get; set; }
    public string Sexo { get; set; } = null!;
    public string? EstadoCivil { get; set; }
    public string? Telefono { get; set; }
    public string? TelefonoSecundario { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
    public string? Ocupacion { get; set; }
    public string? TipoSangre { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; }
    public ICollection<ContactoEmergencia> ContactosEmergencia { get; set; } = new List<ContactoEmergencia>();
}
