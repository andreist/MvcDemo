using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using MvcDemo.Common;


namespace MvcDemo.DAL
{
    public interface IGenericRepository<TEntity> where TEntity: class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        CustomDataSource<TEntity> BindData(string sidx, string sord, int page, int rows, bool search, string filters,
            string secondFilter = "");

        void Insert(TEntity entity);

        void Update(TEntity entityToUpdate);

        void Delete(TEntity entityToDelete);

        void DeleteAll();

    }
}
