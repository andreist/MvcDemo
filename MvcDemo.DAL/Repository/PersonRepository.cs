using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

using MvcDemo.Common;
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

        public CustomDataSource<Person> BindData(string sidx, string sord, int page, int rows, bool search, string filters)
        {
            var serializer = new JavaScriptSerializer();
            var filtersTemp = (!search || string.IsNullOrEmpty(filters)) ? null : serializer.Deserialize<Filters>(filters);

            ObjectContext objectContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)Context).ObjectContext;
            ObjectSet<Person> objectSet = objectContext.CreateObjectSet<Person>();

            ObjectQuery<Person> filteredQuery = filtersTemp == null ? objectSet : filtersTemp.FilterObjectSet(objectSet);

            filteredQuery.MergeOption = MergeOption.NoTracking; // we don't want to update the data

            var totalRecords = filteredQuery.Count();
            var pagedQuery = filteredQuery.Skip("it." + sidx + " " + sord, "@skip", new ObjectParameter("skip", (page - 1) * rows))
                                          .Top("@limit", new ObjectParameter("limit", rows));

            var customDataSource = new CustomDataSource<Person>(pagedQuery.ToList(), totalRecords);
            return customDataSource;
        }


    }
}
