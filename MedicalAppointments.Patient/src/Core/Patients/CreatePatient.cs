using Core.Interfaces.Repository;
using Domain.Model;

namespace Core.Patients;

public sealed class CreatePatientService(IPatients patients)
{
    public async Task<Guid> ExecuteAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        Validate(patient);
        patient.DocumentNumber = patient.DocumentNumber.Trim();

        if (await patients.ExistsByDocumentNumberAsync(patient.DocumentNumber, cancellationToken))
            throw new InvalidOperationException("Ya existe un paciente con ese número de documento.");

        patient.Id = patient.Id == Guid.Empty ? Guid.NewGuid() : patient.Id;
        patient.CreatedAtUtc = DateTime.UtcNow;
        await patients.AddAsync(patient, cancellationToken);
        return patient.Id;
    }

    private static void Validate(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        Require(patient.DocumentNumber, nameof(patient.DocumentNumber), 25);
        Require(patient.FirstName, nameof(patient.FirstName), 100);
        Require(patient.LastName, nameof(patient.LastName), 100);
        Require(patient.Phone, nameof(patient.Phone), 30);
        Require(patient.Email, nameof(patient.Email), 254);
        Require(patient.Address, nameof(patient.Address), 300);
    }

    private static void Require(string? value, string name, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > maxLength)
            throw new ArgumentException($"{name} es obligatorio y no puede superar {maxLength} caracteres.", name);
    }
}
