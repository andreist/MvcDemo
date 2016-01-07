
using AutoMapper;
using MvcDemo.BLL;

namespace MvcDemo.DTO
{
    public static class DtoMappers
    {
        public static void Init()
        {
            Mapper.CreateMap<PersonDto, PersonModel>();
            Mapper.CreateMap<PersonModel, PersonDto>();
        }
    }
}