using MediatR;
using Application.Dtos;

namespace Application.Personas.Commands.UpdatePersona
{
    public record UpdatePersonaCommand(
        int Id,
        string? NombreCompleto,
        int? Edad,
        string? Domicilio,
        string? Telefono,
        string? Profesion
    ) : IRequest<PersonaDto?>;
}
