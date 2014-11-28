using System.Collections;


namespace MvcDemo.DAL
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        Person GetById(int id);
        IList GetIListWithFirstNames(string firstNameContained);
        IList GetIListWithLastNames(string lastNameContained);
        void DeleteById(int id);
    }
}
