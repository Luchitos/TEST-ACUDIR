using Application.Dtos;

using MediatR;

namespace Application.Personas.Queries.GetPersonaById
{
    public record GetPersonaByIdQuery(int Id) : IRequest<PersonaDto?>;
}
