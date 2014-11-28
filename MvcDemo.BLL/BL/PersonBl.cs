using System.Linq;
using System.Collections;

using AutoMapper;
using MvcDemo.DAL;
using MvcDemo.Common;


namespace MvcDemo.BLL
{
    public class PersonBl : GenericBl, IPersonBl
    {
        public PersonBl() { }

        public PersonBl(IUnitOfWork uow) : base(uow){ }


        internal PersonDto FromEntityToModel(Person person)
        {
            var personDto = Mapper.Map<PersonDto>(person);
            return personDto;
        }

        internal Person FromModelToEntity(PersonDto personDto)
        {
            var person = Mapper.Map<Person>(personDto);
            return person;
        }

        public PersonDto GetById(int id)
        {
            Person preson = Uow.PersonRepository.GetById(id);
            if (preson != null)
                return FromEntityToModel(preson);
            return new PersonDto();
        }


        public CustomDataSource<PersonDto> BindData(string sidx, string sord, int page, int rows, bool search, string filters)
        {
            CustomDataSource<Person> personList = Uow.PersonRepository.BindData(sidx, sord, page, rows, search, filters);
            var customDataSource = new CustomDataSource<PersonDto>(personList.RecordList.Select(FromEntityToModel).ToList(),
                personList.TotalRecords);

            return customDataSource;
        }

        public IList GetIListWithFirstNames(string firstNameContained)
        {
            return Uow.PersonRepository.GetIListWithFirstNames(firstNameContained);
        }

        public IList GetIListWithLastNames(string lastNameContained)
        {
            return Uow.PersonRepository.GetIListWithLastNames(lastNameContained);
        }

        public void DeleteById(int id)
        {
            Uow.PersonRepository.DeleteById(id);
        }

        public void Insert(PersonDto personDto)
        {
            Uow.PersonRepository.Insert(FromModelToEntity(personDto));
        }

        public void Update(PersonDto personDto)
        {
            Uow.PersonRepository.Update(FromModelToEntity(personDto));
        }
    }
}
