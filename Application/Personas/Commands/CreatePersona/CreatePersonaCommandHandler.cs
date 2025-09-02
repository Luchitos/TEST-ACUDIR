using AutoMapper;

using Domain.Entidades;
using Domain.Interfaz;

using MediatR;

namespace Application.Personas.Commands.CreatePersona
{
    public class CreatePersonaCommandHandler : IRequestHandler<CreatePersonaCommand, int>
    {
        private readonly IPersonaRepository _personaRepository;
        private readonly IMapper _mapper;

        public CreatePersonaCommandHandler(IPersonaRepository personaRepository, IMapper mapper)
        {
            _personaRepository = personaRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreatePersonaCommand request, CancellationToken cancellationToken)
        {
            var persona = _mapper.Map<Persona>(request);
            await _personaRepository.AddAsync(persona);
            return persona.Id;
        }
    }
}
