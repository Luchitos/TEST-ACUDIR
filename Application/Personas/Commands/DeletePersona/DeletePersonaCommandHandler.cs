using Domain.Interfaz;

using MediatR;

namespace Application.Personas.Commands.DeletePersona
{
    public class DeletePersonaCommandHandler : IRequestHandler<DeletePersonaCommand, bool>
    {
        private readonly IPersonaRepository _personaRepository;

        public DeletePersonaCommandHandler(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public async Task<bool> Handle(DeletePersonaCommand request, CancellationToken cancellationToken)
        {
            var persona = await _personaRepository.GetByIdAsync(request.Id);
            if (persona is null)
                return false;

            // Idealmente usarías un DeleteAsync(persona) si el repo lo expone.
            // Como no esta en IPersonaRepository, se podrìa agregarlo en el futuro.
            persona.NombreCompleto = "[ELIMINADO]";
            await _personaRepository.UpdateAsync(persona);
            return true;
        }
    }
}
