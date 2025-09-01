using Application.Dtos;
using Application.Services;

using MediatR;

namespace Application.Request.PersonaRequest
{
    public class GetPersonaRequest : IRequest<IEnumerable<PersonaDto>>
    {
        public PersonaDto? Persona { get; set; }
    }
    public class GetPersonaRequestHandler : IRequestHandler<GetPersonaRequest, IEnumerable<PersonaDto>>
    {
        private readonly IServicePersona _servicePersona;

        public GetPersonaRequestHandler(IServicePersona servicePersona)
        {
            _servicePersona = servicePersona;
        }

        public async Task<IEnumerable<PersonaDto>> Handle(GetPersonaRequest request, CancellationToken cancellationToken)
        {
            return await _servicePersona.GetPersonasAsync(request.Persona);
        }
    }
}
