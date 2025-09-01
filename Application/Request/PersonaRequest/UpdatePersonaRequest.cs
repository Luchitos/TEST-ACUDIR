using Application.Dtos;
using Application.Services;

using MediatR;

namespace Application.Request.PersonaRequest
{
    public class UpdatePersonaRequest : IRequest<PersonaDto?>
    {
        public UpdatePersonaRequestDto persona { get; set; }
    }
    public class UpdatePersonaRequestHandler : IRequestHandler<UpdatePersonaRequest, PersonaDto?>
    {
        private readonly IServicePersona _servicePersona;

        public UpdatePersonaRequestHandler(IServicePersona servicePersona)
        {
            _servicePersona = servicePersona;
        }

        public async Task<PersonaDto?> Handle(UpdatePersonaRequest request, CancellationToken cancellationToken)
        {
            return await _servicePersona.UpdatePersonaAsync(request.persona);
        }
    }
}
