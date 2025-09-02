using Application.Dtos;

using AutoMapper;

using Domain.Interfaz;

using MediatR;

namespace Application.Personas.Commands.UpdatePersona
{
    public class UpdatePersonaCommandHandler : IRequestHandler<UpdatePersonaCommand, PersonaDto?>
    {
        private readonly IPersonaRepository _personaRepository;
        private readonly IMapper _mapper;

        public UpdatePersonaCommandHandler(IPersonaRepository personaRepository, IMapper mapper)
        {
            _personaRepository = personaRepository;
            _mapper = mapper;
        }

        public async Task<PersonaDto?> Handle(UpdatePersonaCommand request, CancellationToken cancellationToken)
        {
            var persona = await _personaRepository.GetByIdAsync(request.Id);

            if (persona == null)
                return null;

            bool hayCambios = false;

            if (!string.IsNullOrEmpty(request.NombreCompleto) && request.NombreCompleto != persona.NombreCompleto)
            {
                persona.NombreCompleto = request.NombreCompleto;
                hayCambios = true;
            }

            if (request.Edad.HasValue && request.Edad.Value != persona.Edad)
            {
                persona.Edad = request.Edad.Value;
                hayCambios = true;
            }

            if (!string.IsNullOrEmpty(request.Domicilio) && request.Domicilio != persona.Domicilio)
            {
                persona.Domicilio = request.Domicilio;
                hayCambios = true;
            }

            if (!string.IsNullOrEmpty(request.Telefono) && request.Telefono != persona.Telefono)
            {
                persona.Telefono = request.Telefono;
                hayCambios = true;
            }

            if (!string.IsNullOrEmpty(request.Profesion) && request.Profesion != persona.Profesion)
            {
                persona.Profesion = request.Profesion;
                hayCambios = true;
            }

            if (!hayCambios)
                return _mapper.Map<PersonaDto>(persona); // no actualiza pero devuelve

            await _personaRepository.UpdateAsync(persona);
            return _mapper.Map<PersonaDto>(persona);
        }
    }
}
