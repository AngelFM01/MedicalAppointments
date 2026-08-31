using System.Text.Json.Serialization;

namespace Domain.Model;

public class Paciente
{
    [JsonIgnore]
    public long PacienteId { get; set; } = 0;
    [JsonIgnore]
    public string? CodigoPaciente { get; set; } = null!;
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
    [JsonIgnore]
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public ICollection<ContactoEmergencia>? ContactosEmergencia { get; set; } = new List<ContactoEmergencia>();
}
