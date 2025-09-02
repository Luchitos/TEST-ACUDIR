using MediatR;
using Application.Dtos;

namespace Application.Personas.Queries.GetPersonas
{
    public class GetPersonasQuery : IRequest<IEnumerable<PersonaDto>>
    {
        public string? NombreCompleto { get; init; }
        public int? Edad { get; init; }
        public string? Domicilio { get; init; }
        public string? Telefono { get; init; }
        public string? Profesion { get; init; }
    }
}
