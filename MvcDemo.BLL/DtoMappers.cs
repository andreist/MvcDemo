using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MvcDemo.DAL;

namespace MvcDemo.BLL
{
    public static class DtoMappers
    {
        public static void Init()
        {
            Mapper.CreateMap<Person, PersonDto>();
            Mapper.CreateMap<PersonDto, Person>();
        }
    }
}
