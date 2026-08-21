using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Core.Features.FEmergencyContacts.CreatemergencyContacts;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/emergencyContacts")]
    public class EmergencyContactsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public EmergencyContactsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{pacientId}")]
        public async Task<IActionResult> GetEmergencyContacts(Guid pacientId, CancellationToken cancellationToken)
        {
            var query = new Core.Features.FEmergencyContacts.GetEmergencyContactsQuery { PacientID = pacientId };
            var emergencyContacts = await _mediator.Send(query, cancellationToken);
            return Ok(emergencyContacts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmergencyContact(CreateEmergencyContactsCommand command, CancellationToken cancellationToken)
        {
            var emergencyContact = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetEmergencyContacts), new { pacientId = emergencyContact.PacientID }, emergencyContact);
        }
    }
}
