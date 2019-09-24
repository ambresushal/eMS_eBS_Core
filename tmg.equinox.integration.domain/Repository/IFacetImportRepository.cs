using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.domain
{
    public interface IFacetImportRepository<TEntity> where TEntity : Entity
    {
        TEntity FindById(object id);
        TEntity Find(params object[] keyValues);
        void InsertGraph(TEntity entity);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entity);
        FacetRepositoryQuery<TEntity> Query();
        IQueryable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>,
               IOrderedQueryable<TEntity>> orderBy = null,
           List<Expression<Func<TEntity, object>>>
               includeProperties = null,
           int? page = null,
           int? pageSize = null,
            bool trackChanges = true);
        string SqlQuery(string sql, params object[] parameters);
        IQueryable<TEntity> SqlQueryFacet(string sql, params object[] parameters);
        IList<TEntity> SqlQueryFacet1(string sql, params object[] parameters);
    }
}
