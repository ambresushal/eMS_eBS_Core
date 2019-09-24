using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using tmg.equinox.integration.data;
using tmg.equinox.integration.domain.Impl;

namespace tmg.equinox.integration.domain
{
    public interface IFacetRepository<TEntity> where TEntity : Entity
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
        void SqlQueryFacetStepMaster(int processGovernanceId);
        void SqlQueryFacetMLStepMaster(int processGovernanceId);
        void SqlQueryFacetMLStepMasterML(int processGovernanceId, string section);
        void SqlQueryFacetMLTStepMaster(int processGovernanceId, string environment);
        MultipleResultSetWrapper MultipleResultSets(string storedProcedure, params object[] parameters);
        void SqlQuerySP(string sql, params object[] parameters);
        void SetContext(IDbContext context);
    }
}
