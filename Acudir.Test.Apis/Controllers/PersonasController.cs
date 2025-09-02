using Application.Common.Pagination;
using Application.Dtos;
using Application.Personas.Commands.CreatePersona;
using Application.Personas.Commands.DeletePersona;
using Application.Personas.Commands.UpdatePersona;
using Application.Personas.Queries.GetPersonaById;
using Application.Personas.Queries.GetPersonas;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acudir.Test.Apis.Controllers
{
    /// <summary>
    /// Controlador de gestión de personas.
    /// </summary>
    /// <remarks>
    /// Todas las rutas están versionadas bajo /api/v1/personas.
    /// Requiere autorización JWT.
    /// </remarks>
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class PersonasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Obtiene todas las personas con filtros y paginación.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<PersonaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromQuery] GetPersonasQuery query, CancellationToken ct = default)
        {
            var page = await _mediator.Send(query, ct).ConfigureAwait(false);

            if (page.TotalItems == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "Sin resultados",
                    Detail = "No se encontraron personas con los datos proporcionados.",
                    Status = StatusCodes.Status404NotFound
                });

            return Ok(page);
        }


        /// <summary>Obtiene una persona por ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PersonaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetPersonaByIdQuery(id));
            return result is null
                ? NotFound(new ProblemDetails
                {
                    Title = "Persona no encontrada",
                    Detail = $"No existe una persona con ID = {id}",
                    Status = StatusCodes.Status404NotFound
                })
                : Ok(result);
        }

        /// <summary>Agrega una nueva persona.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> Post([FromBody] CreatePersonaCommand command, CancellationToken ct = default)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>Actualiza una persona existente.</summary>
        [HttpPut]
        [ProducesResponseType(typeof(PersonaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromBody] UpdatePersonaCommand command, CancellationToken ct = default)
        {
            var result = await _mediator.Send(command);
            return result is null
                ? NotFound(new ProblemDetails
                {
                    Title = "Persona no encontrada",
                    Detail = $"No se encontró la persona con ID = {command.Id}",
                    Status = StatusCodes.Status404NotFound
                })
                : Ok(result);
        }

        /// <summary>Elimina una persona por ID.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new DeletePersonaCommand(id));
            return !result
                ? NotFound(new ProblemDetails
                {
                    Title = "Persona no encontrada",
                    Detail = $"No existe una persona con ID = {id}",
                    Status = StatusCodes.Status404NotFound
                })
                : NoContent();
        }
    }

}
