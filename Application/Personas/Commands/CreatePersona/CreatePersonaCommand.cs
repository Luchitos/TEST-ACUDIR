using MediatR;

namespace Application.Personas.Commands.CreatePersona
{
    public record CreatePersonaCommand(
        string NombreCompleto,
        int Edad,
        string Domicilio,
        string Telefono,
        string Profesion
    ) : IRequest<int>;
}
