using Application.Dtos;

using AutoMapper;

using Domain.Interfaz;

using MediatR;

namespace Application.Personas.Queries.GetPersonas
{
    public class GetPersonasQueryHandler : IRequestHandler<GetPersonasQuery, IEnumerable<PersonaDto>>
    {
        private readonly IPersonaRepository _personaRepository;
        private readonly IMapper _mapper;

        public GetPersonasQueryHandler(IPersonaRepository personaRepository, IMapper mapper)
        {
            _personaRepository = personaRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PersonaDto>> Handle(GetPersonasQuery request, CancellationToken cancellationToken)
        {
            var personas = await _personaRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(request.NombreCompleto))
                personas = personas.Where(p => p.NombreCompleto.Contains(request.NombreCompleto, StringComparison.OrdinalIgnoreCase));

            if (request.Edad.HasValue)
                personas = personas.Where(p => p.Edad == request.Edad.Value);

            if (!string.IsNullOrEmpty(request.Domicilio))
                personas = personas.Where(p => p.Domicilio.Contains(request.Domicilio, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(request.Telefono))
                personas = personas.Where(p => p.Telefono.Contains(request.Telefono, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(request.Profesion))
                personas = personas.Where(p => p.Profesion.Contains(request.Profesion, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<IEnumerable<PersonaDto>>(personas);
        }
    }
}
