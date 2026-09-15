using MedicalAppointments.Core.Interfaces.Repository;
using MedicalAppointments.Domain.Model;
using MediatR;
using Nuget_Persistence.Filtering;
using Nuget_Persistence.Models;

namespace MedicalAppointments.Core.Feature.EmergencyContacts.Queries;

public sealed class GetEmergencyContactsPagedQuery : IRequest<PagedResult<ContactoEmergencia>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    /// <summary>Filtro dinámico estilo Dynamic LINQ, ej: PacienteId == 1 AND Activo == true</summary>
    public string? Filter { get; set; }
}

public sealed class GetEmergencyContactsPagedQueryHandler(IContactosEmergenciaRepository contactsRepository)
    : IRequestHandler<GetEmergencyContactsPagedQuery, PagedResult<ContactoEmergencia>>
{
    public Task<PagedResult<ContactoEmergencia>> Handle(GetEmergencyContactsPagedQuery request, CancellationToken cancellationToken)
    {
        var filterExpression = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<ContactoEmergencia>(request.Filter);

        return contactsRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filterExpression,
            orderBy: query => query.OrderBy(x => x.PacienteId).ThenBy(x => x.Prioridad),
            cancellationToken: cancellationToken);
    }
}
