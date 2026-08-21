using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;

namespace Core.Features.FEmergencyContacts
{
    public record GetEmergencyContactsQuery : IRequest<List<EmergencyContacts>>
    {
        public Guid PacientID { get; set; }
    }
    public class GetEmergencyContactsHandler : IRequestHandler<GetEmergencyContactsQuery, List<EmergencyContacts>>
    {
        private readonly IEmergencyContactsRepository _emergencyContactsRepository;
        public GetEmergencyContactsHandler(IEmergencyContactsRepository emergencyContactsRepository)
        {
            _emergencyContactsRepository = emergencyContactsRepository;
        }
        public async Task<List<EmergencyContacts>> Handle(GetEmergencyContactsQuery request, CancellationToken cancellationToken)
        {
            var emergencyContacts = await _emergencyContactsRepository.GetContact();
            return emergencyContacts.Where(ec => ec.PacientID == request.PacientID).ToList();
        }
    }
}
