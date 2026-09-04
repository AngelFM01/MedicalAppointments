using System.ComponentModel.DataAnnotations.Schema;
using Domain.Abstractions;

namespace Domain.Model;

public class ContactoEmergencia : IEntity<long>
{
    public long ContactoEmergenciaId { get; set; }

    /// <summary>Alias genérico de la clave primaria exigido por <see cref="IEntity{TKey}"/>.
    /// No se mapea a columna: el repositorio genérico resuelve la clave real por metadatos de EF Core.</summary>
    [NotMapped]
    public long Id => ContactoEmergenciaId;
    public long PacienteId { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string? Parentesco { get; set; }
    public string Telefono { get; set; } = null!;
    public string? TelefonoSecundario { get; set; }
    public string? Email { get; set; }
    public int Prioridad { get; set; } = 1;
    public bool Activo { get; set; } = true;
    public Paciente Paciente { get; set; } = null!;
}
