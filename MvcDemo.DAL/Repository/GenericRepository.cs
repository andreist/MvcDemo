using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.Entity.Core.Objects;
using System.Web.Script.Serialization;

using MvcDemo.Common;



namespace MvcDemo.DAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal MvcDemoEntities Context;
        internal DbSet<TEntity> ObjectSet;

        public GenericRepository(MvcDemoEntities context)
        {
            Context = context;
            ObjectSet = Context.Set<TEntity>();
        }

        internal virtual IQueryable<TEntity> GetIQueryable()
        {
            IQueryable<TEntity> query = ObjectSet;
            return query;
        } 
            

        /// <summary>
        /// Get a IEnumerable of entities: filtered, ordered and which include related entities
        /// ex: Get( filter: m => m.Property1 > 10, orderBy: m => m.OrderBy(s => s.Property2), includeProperties: "Table1")
        /// </summary>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = ObjectSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includepropery in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includepropery);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }

        /// <summary>
        /// Get a CustomDataSource: filtered, ordered
        /// ex: secondFilter = "it.SectionID = @SectionId"
        /// </summary>
        public CustomDataSource<TEntity> BindData(string sidx, string sord, int page, int rows, bool search, string filters, string secondFilter = "")
        {
            var serializer = new JavaScriptSerializer();
            var filtersTemp = (!search || string.IsNullOrEmpty(filters)) ? null : serializer.Deserialize<Filters>(filters);

            ObjectContext objectContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)Context).ObjectContext;
            ObjectSet<TEntity> objectSet = objectContext.CreateObjectSet<TEntity>();

            ObjectQuery<TEntity> filteredQuery = filtersTemp == null ? objectSet : filtersTemp.FilterObjectSet(objectSet);

            filteredQuery.MergeOption = MergeOption.NoTracking; // we don't want to update the data

            if (!string.IsNullOrEmpty(secondFilter))
                filteredQuery = filteredQuery.Where(secondFilter);

            var totalRecords = filteredQuery.Count();
            var pagedQuery = filteredQuery.Skip("it." + sidx + " " + sord, "@skip", new ObjectParameter("skip", (page - 1) * rows))
                                          .Top("@limit", new ObjectParameter("limit", rows));

            var customDataSource = new CustomDataSource<TEntity>(pagedQuery.ToList(), totalRecords);
            return customDataSource;
        }

        public virtual void Insert(TEntity entity)
        {
            ObjectSet.Add(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            if (Context.Entry(entityToUpdate).State == EntityState.Detached)
                ObjectSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                ObjectSet.Attach(entityToDelete);
            }
            ObjectSet.Remove(entityToDelete);
        }

        public virtual void DeleteAll()
        {
            foreach (TEntity entity in ObjectSet.ToList())
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    ObjectSet.Attach(entity);
                }
                ObjectSet.Remove(entity);
            }
        }
    }
}