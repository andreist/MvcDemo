using System.Collections.Generic;
using System.Linq;
using MvcDemo.DAL;
using MvcDemo.Common;
using System.Collections;


namespace MvcDemo.BLL
{
    public class PersonBl : IPersonBl
    {
        internal PersonDto FromEntityToModel(Person person)
        {
            PersonDto personDto = new PersonDto()
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Age = person.Age,
                Sex = person.Sex
            };
            return personDto;
        }

        internal Person FromModelToEntity(PersonDto personDto)
        {
            Person person = new Person()
            {
                Id = personDto.Id,
                FirstName = personDto.FirstName,
                LastName = personDto.LastName,
                Age = personDto.Age,
                Sex = personDto.Sex
            };
            return person;
        }

        public PersonDto GetById(int id)
        {
            Person preson = UnitOfWork.Current.PersonRepository.GetById(id);
            if (preson != null)
                return FromEntityToModel(preson);
            return new PersonDto();
        }


        public CustomDataSource<PersonDto> BindData(string sidx, string sord, int page, int rows, bool search, string filters)
        {
            CustomDataSource<Person> personList = UnitOfWork.Current.PersonRepository.BindData(sidx, sord, page, rows, search, filters);
            var customDataSource = new CustomDataSource<PersonDto>(personList.RecordList.Select(FromEntityToModel).ToList(),
                personList.TotalRecords);

            return customDataSource;
        }

        public IList GetIListWithFirstNames(string firstNameContained)
        {
            return UnitOfWork.Current.PersonRepository.GetIListWithFirstNames(firstNameContained);
        }

        public IList GetIListWithLastNames(string lastNameContained)
        {
            return UnitOfWork.Current.PersonRepository.GetIListWithLastNames(lastNameContained);
        }

        public void DeleteById(int id)
        {
            UnitOfWork.Current.PersonRepository.DeleteById(id);
        }

        public void Insert(PersonDto personDto)
        {
            UnitOfWork.Current.PersonRepository.Insert(FromModelToEntity(personDto));
        }

        public void Update(PersonDto personDto)
        {
            UnitOfWork.Current.PersonRepository.Update(FromModelToEntity(personDto));
        }
    }
}
