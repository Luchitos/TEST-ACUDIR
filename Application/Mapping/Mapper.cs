using Application.Dtos;
using Application.Personas.Commands.CreatePersona;

using AutoMapper;

using Domain.Entidades;

namespace Application.Mapping
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Persona, PersonaDto>().ReverseMap();
            CreateMap<CreatePersonaCommand, Persona>();
        }
    }
}
