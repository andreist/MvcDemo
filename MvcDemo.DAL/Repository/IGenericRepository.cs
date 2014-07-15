using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MvcDemo.DAL.Repository
{
    public interface IGenericRepository<TEntity> where TEntity: class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        void Insert(TEntity entity);

        void Update(TEntity entityToUpdate);

        void Delete(TEntity entityToDelete);

        void DeleteAll();

    }
}
