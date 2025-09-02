using Application.Common.Pagination;
using Application.Dtos;

using MediatR;

namespace Application.Personas.Queries.GetPersonas;

public sealed record GetPersonasQuery : IRequest<PagedResult<PersonaDto>>
{
    // Paging
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 20;

    // Sorting
    public string? SortBy { get; init; } = "Id";
    public string SortDir { get; init; } = "asc";

    // Filters
    public string? Nombre { get; init; }
    public int? Edad { get; init; }
    public int? MinEdad { get; init; }
    public int? MaxEdad { get; init; }
    public string? Domicilio { get; init; }
    public string? Telefono { get; init; }
    public string? Profesion { get; init; }
}
