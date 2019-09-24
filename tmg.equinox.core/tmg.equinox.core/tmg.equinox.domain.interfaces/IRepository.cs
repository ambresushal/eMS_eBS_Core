using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;

namespace tmg.equinox.repository.interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        TEntity FindById(object id);
        TEntity Find(params object[] keyValues);
        void InsertGraph(TEntity entity);
        void Update(TEntity entity);
        void UpdateCollateralEntity(TEntity entity);
        void Update(TEntity entity, bool flag);
        void Delete(object id);
        //void DeleteByID(int id);
        //void DeleteByID(int[] idList);
        void Delete(TEntity entity);
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entity);
        void DeleteRange(IEnumerable<TEntity> entityList);
        void DeleteRange(Expression<Func<TEntity, bool>> filter);
        void DeleteSql(int[] keyValues);
        RepositoryQuery<TEntity> Query();
        IQueryable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>,
               IOrderedQueryable<TEntity>> orderBy = null,
           List<Expression<Func<TEntity, object>>>
               includeProperties = null,
           int? page = null,
           int? pageSize = null,
            bool trackChanges = true);
        IQueryable<TEntity> ExecuteSql(string sql, params object[] parameters);

        int ExecuteUpdateSql(string sql, params object[] parameters);
    }
}
