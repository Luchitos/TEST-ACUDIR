using Application.Dtos;

namespace Application.Services
{
    public interface IServicePersona
    {
        Task<IEnumerable<PersonaDto>> GetPersonasAsync(PersonaDto? filter);
        Task<PersonaDto> AddPersonaAsync(AddPersonaRequestDto addPersonaDto);
        Task<PersonaDto?> UpdatePersonaAsync(UpdatePersonaRequestDto updatePersonaDto);
    }
}
