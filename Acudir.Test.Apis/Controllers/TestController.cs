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
    [Route("Personas")]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IServicePersona _servicePersona;

        public TestController(IMediator mediator, IServicePersona servicePersona)
        {
            _mediator = mediator;
            _servicePersona = servicePersona;
        }

        #region GetPersonas
        /// <summary>
        /// Obtiene una lista de personas según los parámetros de la consulta. Si no hay parametros, trae la lista completa
        /// </summary>
        [HttpGet("GetPersonas")]
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
                GetPersonaRequest request = new()
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
                IEnumerable<PersonaDto> result = await _mediator.Send(request);

                if (result == null || !result.Any())
                {
                    return NotFound("No se encontraron personas con los datos proporcionados.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener personas: {ex.Message}");
            }
        }
        #endregion

        #region AgregarPersona
        /// <summary>
        /// Agrega una nueva persona.
        /// </summary>
        [HttpPost("AgregarPersona")]
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
                AddPersonaRequestDto addPersonaDto = new()
                {
                    NombreCompleto = nombreCompleto,
                    Edad = edad,
                    Domicilio = domicilio,
                    Telefono = telefono,
                    Profesion = profesion
                };

                PersonaDto result = await _servicePersona.AddPersonaAsync(addPersonaDto);
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
        #endregion

        #region ActualizarPersona
        /// <summary>
        /// Actualiza las propiedades de una persona existente dependiendo del valor de los parámetros. Los parámetros vacíos son ignorados.
        /// </summary>
        [HttpPut("ActualizarPersona")]
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
                UpdatePersonaRequestDto updatePersonaDto = new()
                {
                    Id = id,
                    NombreCompleto = nombreCompleto,
                    Edad = edad,
                    Domicilio = domicilio,
                    Telefono = telefono,
                    Profesion = profesion
                };

                PersonaDto? result = await _servicePersona.UpdatePersonaAsync(updatePersonaDto);

                if (result == null)
                {
                    return BadRequest("No se detectaron cambios.");
                }

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
        #endregion
    }
}

