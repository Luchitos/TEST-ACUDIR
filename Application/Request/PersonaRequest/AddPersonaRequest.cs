using Application.Dtos;
using Application.Services;

using MediatR;

namespace Application.Request.PersonaRequest
{
    public class AddPersonaRequest : IRequest<PersonaDto>
    {
        public AddPersonaRequestDto persona { get; set; }
    }
    public class AddPersonaRequestHandler : IRequestHandler<AddPersonaRequest, PersonaDto>
    {
        private readonly IServicePersona _servicePersona;

        public AddPersonaRequestHandler(IServicePersona servicePersona)
        {
            _servicePersona = servicePersona;
        }

        public async Task<PersonaDto> Handle(AddPersonaRequest request, CancellationToken cancellationToken)
        {
            return await _servicePersona.AddPersonaAsync(request.persona);
        }
    }
}
