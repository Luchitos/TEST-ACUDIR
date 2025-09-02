using MediatR;

namespace Application.Personas.Commands.DeletePersona
{
    public record DeletePersonaCommand(int Id) : IRequest<bool>;
}
