using Application.Dtos;

using AutoMapper;

using Domain.Interfaz;

using MediatR;

namespace Application.Personas.Queries.GetPersonaById
{
    public class GetPersonaByIdQueryHandler : IRequestHandler<GetPersonaByIdQuery, PersonaDto?>
    {
        private readonly IPersonaRepository _personaRepository;
        private readonly IMapper _mapper;

        public GetPersonaByIdQueryHandler(IPersonaRepository personaRepository, IMapper mapper)
        {
            _personaRepository = personaRepository;
            _mapper = mapper;
        }

        public async Task<PersonaDto?> Handle(GetPersonaByIdQuery request, CancellationToken cancellationToken)
        {
            var persona = await _personaRepository.GetByIdAsync(request.Id);
            return persona is null ? null : _mapper.Map<PersonaDto>(persona);
        }
    }
}
