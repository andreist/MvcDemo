using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using MvcDemo.DAL.Repository;


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