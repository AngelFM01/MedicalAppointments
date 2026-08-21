namespace Domain.Model;

public class ContactoEmergencia
{
    public long ContactoEmergenciaId { get; set; }
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
