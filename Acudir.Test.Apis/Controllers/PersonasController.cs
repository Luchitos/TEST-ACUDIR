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

        /// <summary>Obtiene todas las personas filtrando por campos opcionales.</summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<PersonaDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromQuery] GetPersonasQuery query)
        {
            IEnumerable<PersonaDto> result = await _mediator.Send(query).ConfigureAwait(false);

            // devolver 404 si no hay resultados (lo que tu test espera)
            if (result is null || !result.Any())
                return NotFound("No se encontraron personas con los datos proporcionados.");

            return Ok(result);
        }


        /// <summary>Obtiene una persona por ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PersonaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
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
        public async Task<IActionResult> Post([FromBody] CreatePersonaCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>Actualiza una persona existente.</summary>
        [HttpPut]
        [ProducesResponseType(typeof(PersonaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromBody] UpdatePersonaCommand command)
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
        public async Task<IActionResult> Delete(int id)
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
