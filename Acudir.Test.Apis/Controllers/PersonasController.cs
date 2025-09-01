using Application.Dtos;
using Application.Request.PersonaRequest;
using Application.Services;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace Acudir.Test.Apis.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/personas")]
    public class PersonasController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IServicePersona _servicePersona;

        public PersonasController(IMediator mediator, IServicePersona servicePersona)
        {
            _mediator = mediator;
            _servicePersona = servicePersona;
        }

        // GET /api/v1/personas
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<PersonaDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? id,
            [FromQuery] string? nombre,
            [FromQuery] int? edad,
            [FromQuery] string? domicilio,
            [FromQuery] string? telefono,
            [FromQuery] string? profesion)
        {
            try
            {
                var request = new GetPersonaRequest
                {
                    Persona = new PersonaDto
                    {
                        Id = id ?? 0,
                        NombreCompleto = nombre,
                        Edad = edad ?? 0,
                        Domicilio = domicilio,
                        Telefono = telefono,
                        Profesion = profesion
                    }
                };
                var result = await _mediator.Send(request);
                if (result == null || !result.Any())
                    return NotFound("No se encontraron personas con los datos proporcionados.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener personas: {ex.Message}");
            }
        }

        // POST /api/v1/personas
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(PersonaDto), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Add(
            [FromQuery] string nombreCompleto,
            [FromQuery] int edad,
            [FromQuery] string domicilio,
            [FromQuery] string telefono,
            [FromQuery] string profesion)
        {
            try
            {
                var addPersonaDto = new AddPersonaRequestDto
                {
                    NombreCompleto = nombreCompleto,
                    Edad = edad,
                    Domicilio = domicilio,
                    Telefono = telefono,
                    Profesion = profesion
                };
                var result = await _servicePersona.AddPersonaAsync(addPersonaDto);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al agregar persona: {ex.Message}");
            }
        }

        // PUT /api/v1/personas
        [HttpPut]
        [Authorize]
        [ProducesResponseType(typeof(PersonaDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(
            [FromQuery] int id,
            [FromQuery] string? nombreCompleto,
            [FromQuery] int? edad,
            [FromQuery] string? domicilio,
            [FromQuery] string? telefono,
            [FromQuery] string? profesion)
        {
            try
            {
                var updatePersonaDto = new UpdatePersonaRequestDto
                {
                    Id = id,
                    NombreCompleto = nombreCompleto,
                    Edad = edad,
                    Domicilio = domicilio,
                    Telefono = telefono,
                    Profesion = profesion
                };
                var result = await _servicePersona.UpdatePersonaAsync(updatePersonaDto);
                if (result == null)
                    return BadRequest("No se detectaron cambios.");
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar persona: {ex.Message}");
            }
        }
    }
}
