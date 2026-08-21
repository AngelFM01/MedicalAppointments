namespace Domain.Model;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DocumentNumber { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public Sex Sex { get; set; }
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public MaritalStatus MaritalStatus { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<EmergencyContacts> EmergencyContacts { get; set; } = new List<EmergencyContacts>();
}

public enum Sex { Female = 1, Male = 2, Other = 3 }
public enum MaritalStatus { Single = 1, Married = 2, Divorced = 3, Widowed = 4, CommonLaw = 5 }
