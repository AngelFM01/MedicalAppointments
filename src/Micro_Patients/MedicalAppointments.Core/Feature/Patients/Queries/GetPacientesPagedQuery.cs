using MedicalAppointments.Core.Interfaces.Repository;
using MedicalAppointments.Domain.Model;
using MediatR;
using Nuget_Persistence.Filtering;
using Nuget_Persistence.Models;

namespace MedicalAppointments.Core.Feature.Patients.Queries;

public sealed class GetPacientesPagedQuery : IRequest<PagedResult<Paciente>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    /// <summary>Filtro dinámico estilo Dynamic LINQ, ej: Apellidos.Contains("Perez")</summary>
    public string? Filter { get; set; }
}

public sealed class GetPacientesPagedQueryHandler(IPatientsRepository patientsRepository)
    : IRequestHandler<GetPacientesPagedQuery, PagedResult<Paciente>>
{
    public Task<PagedResult<Paciente>> Handle(GetPacientesPagedQuery request, CancellationToken cancellationToken)
    {
        var filterExpression = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Paciente>(request.Filter);

        return patientsRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filterExpression,
            orderBy: query => query.OrderBy(x => x.Apellidos).ThenBy(x => x.Nombres),
            cancellationToken: cancellationToken);
    }
}
