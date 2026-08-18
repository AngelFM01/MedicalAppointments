using Core.Patients;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController(CreatePatientService createPatientService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(Patient patient, CancellationToken cancellationToken)
    {
        try
        {
            var id = await createPatientService.ExecuteAsync(patient, cancellationToken);
            return Created($"api/patients/{id}", new { id });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message, Status = StatusCodes.Status400BadRequest });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Detail = exception.Message, Status = StatusCodes.Status409Conflict });
        }
    }
}
