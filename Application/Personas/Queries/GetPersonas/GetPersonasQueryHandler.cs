using Application.Common.Pagination;
using Application.Dtos;
using Application.Personas.Specifications;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Domain.Interfaz;

using MediatR;

namespace Application.Personas.Queries.GetPersonas;

public sealed class GetPersonasQueryHandler : IRequestHandler<GetPersonasQuery, PagedResult<PersonaDto>>
{
    private readonly IPersonaRepository _repo;
    private readonly IMapper _mapper;

    public GetPersonasQueryHandler(IPersonaRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<PagedResult<PersonaDto>> Handle(GetPersonasQuery request, CancellationToken ct)
    {
        // El repo trae todo desde Test.json. Aplicamos spec en memoria (IQueryable).
        var personas = await _repo.GetAllAsync(ct); // List<Persona>
        var queryable = personas.AsQueryable();

        var spec = new PersonaByFiltersSpec(
            request.Nombre, request.Edad, request.MinEdad, request.MaxEdad,
            request.Domicilio, request.Telefono, request.Profesion,
            request.SortBy, request.SortDir, request.Page, request.Size);

        var filteredSorted = spec.ApplyFiltersAndSort(queryable);
        var total = filteredSorted.Count();

        var pageItems = spec.ApplyPaging(filteredSorted).ToList();

        var dtoItems = pageItems
            .Select(p => _mapper.Map<PersonaDto>(p))
            .ToList();

        var totalPages = (int)Math.Ceiling(total / (double)request.Size);

        return new PagedResult<PersonaDto>(
            dtoItems,
            request.Page,
            request.Size,
            total,
            totalPages,
            request.SortBy,
            request.SortDir
        );
    }
}
