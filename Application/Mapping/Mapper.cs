using Application.Dtos;

using AutoMapper;

using Domain.Entidades;

namespace Application.Mapping
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<AddPersonaRequestDto, Persona>();
            CreateMap<UpdatePersonaRequestDto, Persona>();
            CreateMap<Persona, PersonaDto>();
        }
    }
}
