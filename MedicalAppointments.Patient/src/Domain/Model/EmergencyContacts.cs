using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Model
{
    public class EmergencyContacts
    {
        [Key]
        public int IdContactEmerg { get; set; }
        [Required]
        public Guid PacientID { get; set; }

        [Required]
        [MaxLength(200)]
        public string NombreCompleto { get; set; }
        public string Parentesco { get; set; }
        public string Telefono { get; set; }
        public string TelefonoAlternativo { get; set; }
        public string Email { get; set; }
        [Required]
        // VALIDACIÓN EN C# para la Prioridad (equivalente al CHECK)
        [Range(1, int.MaxValue, ErrorMessage = "La prioridad debe ser mayor a 0")]
        public int Prioridad { get; set; } = 1;

        [Required]
        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }
        public required string usuarioCreacion { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(PacientID))] // Atributo puesto en la navegación
        public virtual Patient Paciente { get; set; } = null!;





    }
}
