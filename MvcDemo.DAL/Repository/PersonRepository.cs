using System.Linq;
using System.Collections;

namespace MvcDemo.DAL
{
    public class PersonRepository : GenericRepository<Person>
    {
        public PersonRepository(MvcDemoEntities context)
            : base(context)
        { }

        public Person GetById(int id)
        {
            return ObjectSet.FirstOrDefault(m => m.Id == id);
        }

        public IList GetIListWithFirstNames(string firstNameContained)
        {
            return ObjectSet.Where(m => m.FirstName.Contains(firstNameContained))
                                      .OrderBy(o => o.FirstName)
                                      .Select(s => s.FirstName)
                                      .ToList();
        }

        public IList GetIListWithLastNames(string lastNameContained)
        {
            return ObjectSet.Where(m => m.LastName.Contains(lastNameContained))
                                      .OrderBy(o => o.LastName)
                                      .Select(s => s.LastName)
                                      .ToList();
        }

        public void DeleteById(int id)
        {
            Person item = GetById(id);
            if (item != null)
                Delete(item);
        }
    }
}
