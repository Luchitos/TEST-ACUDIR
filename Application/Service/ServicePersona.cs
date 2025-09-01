using Application.Dtos;

using AutoMapper;

using Domain.Entidades;
using Domain.Interfaz;

using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class ServicePersona : IServicePersona
    {
        private readonly IPersonaRepository _personaRepository;
        private readonly IMapper _mapper;

        public ServicePersona(IPersonaRepository personaRepository, IMapper mapper)
        {
            _personaRepository = personaRepository;
            _mapper = mapper;
        }
        #region GetPersonaRequest
        public async Task<IEnumerable<PersonaDto>> GetPersonasAsync(PersonaDto? filter)
        {
            var personas = await _personaRepository.GetAllAsync();

            if (filter != null)
            {
                if (filter.Id != 0)
                {
                    personas = personas.Where(p => p.Id == filter.Id);
                }

                if (!string.IsNullOrEmpty(filter.NombreCompleto))
                {
                    personas = personas.Where(p => p.NombreCompleto.Contains(filter.NombreCompleto));
                }

                if (filter.Edad != 0)
                {
                    personas = personas.Where(p => p.Edad == filter.Edad);
                }

                if (!string.IsNullOrEmpty(filter.Domicilio))
                {
                    personas = personas.Where(p => p.Domicilio.Contains(filter.Domicilio));
                }

                if (!string.IsNullOrEmpty(filter.Telefono))
                {
                    personas = personas.Where(p => p.Telefono.Contains(filter.Telefono));
                }

                if (!string.IsNullOrEmpty(filter.Profesion))
                {
                    personas = personas.Where(p => p.Profesion.Contains(filter.Profesion));
                }
            }

            return _mapper.Map<IEnumerable<PersonaDto>>(personas);
        }
        #endregion

        #region AddPersonaRequest
        public async Task<PersonaDto> AddPersonaAsync(AddPersonaRequestDto addPersonaDto)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(addPersonaDto);
            if (!Validator.TryValidateObject(addPersonaDto, validationContext, validationResults, true))
            {
                throw new ValidationException("Validation failed for AddPersonaRequestDto");
            }

            var persona = _mapper.Map<Persona>(addPersonaDto);
            await _personaRepository.AddAsync(persona);
            return _mapper.Map<PersonaDto>(persona);
        }
        #endregion

        #region UpdatePersonaRequest
        public async Task<PersonaDto?> UpdatePersonaAsync(UpdatePersonaRequestDto updatePersonaDto)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(updatePersonaDto);
            if (!Validator.TryValidateObject(updatePersonaDto, validationContext, validationResults, true))
            {
                throw new ValidationException("Validation failed for UpdatePersonaRequestDto");
            }

            var personaExistente = await _personaRepository.GetByIdAsync(updatePersonaDto.Id);

            if (personaExistente == null)
            {
                return null;
            }

            bool hayCambios = false;
            if (!string.IsNullOrEmpty(updatePersonaDto.NombreCompleto))
            {
                personaExistente.NombreCompleto = updatePersonaDto.NombreCompleto;
                hayCambios = true;
            }
            if (updatePersonaDto.Edad != null)
            {
                personaExistente.Edad = updatePersonaDto.Edad.Value;
                hayCambios = true;
            }
            if (!string.IsNullOrEmpty(updatePersonaDto.Domicilio))
            {
                personaExistente.Domicilio = updatePersonaDto.Domicilio;
                hayCambios = true;
            }
            if (!string.IsNullOrEmpty(updatePersonaDto.Telefono))
            {
                personaExistente.Telefono = updatePersonaDto.Telefono;
                hayCambios = true;
            }
            if (!string.IsNullOrEmpty(updatePersonaDto.Profesion))
            {
                personaExistente.Profesion = updatePersonaDto.Profesion;
                hayCambios = true;
            }

            if (!hayCambios)
            {
                return null;
            }

            await _personaRepository.UpdateAsync(personaExistente);
            return _mapper.Map<PersonaDto>(personaExistente);
        }
        #endregion
    }
}

