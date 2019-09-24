using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;
using System.Reflection;
using System.Runtime;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using System.Text.RegularExpressions;
using tmg.equinox.repository.Repository;


namespace tmg.equinox.repository.interfaces
{
    public class Repository<TEntity> : IDisposable, IRepositoryAsync<TEntity> where TEntity : Entity
    {
        private IDbContextAsync Context;
        private DbSet<TEntity> DbSet;

        public Repository(IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Context = (IDbContextAsync)context;

            var dbContext = Context as DbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        public Repository(IDbContextAsync context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Context = context;

            var dbContext = Context as DbContext;
            DbSet = dbContext.Set<TEntity>();
        }
        public virtual TEntity FindById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            return DbSet.Find(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await DbSet.FindAsync(keyValues);
        }


        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await DbSet.FindAsync(cancellationToken, keyValues);
        }

        public virtual void InsertGraph(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void UpdateCollateralEntity(TEntity entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public virtual void Update(TEntity entity, bool flag)
        {
            if (flag == true)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    TEntity attachedEntity = DbSet.Local.FirstOrDefault(e => e.Id == entity.Id);

                    if (attachedEntity != null)
                    {
                        var attachedEntry = Context.Entry(attachedEntity);
                        attachedEntry.CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        Context.Entry(entity).State = EntityState.Modified;
                    }
                }
            }
            else
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }
                Context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void Delete(object id)
        {
            var entity = DbSet.Find(id);

            Context.Entry(entity).State = EntityState.Deleted;

            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entityList)
        {
            foreach (var entity in entityList)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }
            }
            DbSet.RemoveRange(entityList);
        }

        public virtual void DeleteRange(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var entity in query)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }
            }

            DbSet.RemoveRange(query);
        }

        public virtual void DeleteSql(int[] keyValues)
        {
            var dbcontext = Context as DbContext;
            var objectContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
            //Get the entity object
            var entity = DbSet.Create<TEntity>();
            //Get type of entity
            var entityType = typeof(TEntity);
            //Get Entity Container object for specified entity container name and the data model
            var entityContainer = objectContext.MetadataWorkspace.GetEntityContainer(objectContext.DefaultContainerName, DataSpace.CSpace);

            if (entityContainer != null)
            {
                //get the name of the entity set that the type T belongs to - e.g. "Cars"
                var baseEntitySet = entityContainer.BaseEntitySets.FirstOrDefault(b => b.ElementType.Name == entityType.Name);
                if (baseEntitySet != null)
                {
                    var entitySetName = baseEntitySet.Name;
                    //finding the entity key of this entity - e.g. Car.ID
                    var entityKey = objectContext.CreateEntityKey(entitySetName, entity);
                    if (entityKey != null)
                    {
                        //Get primary key of selected entity
                        var primaryKey = entityKey.EntityKeyValues[0];
                        if (primaryKey != null)
                        {
                            if (keyValues.Count() > 0)
                            {
                                StringBuilder sqlBuilder = new StringBuilder();
                                sqlBuilder.Append("DELETE FROM ");
                                sqlBuilder.Append(dbcontext.GetTableName<TEntity>());
                                sqlBuilder.Append(" WHERE " + primaryKey.Key + " IN (");
                                sqlBuilder.Append("{0}");
                                sqlBuilder.Append(")");

                                var parms = keyValues.Select((s, i) => "@p" + i.ToString()).ToArray();
                                var inclause = string.Join(",", keyValues);

                                dbcontext.Database.ExecuteSqlCommand(string.Format(sqlBuilder.ToString(), inclause), parms);
                            }
                        }
                    }
                }
            }

        }

        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await DeleteAsync(CancellationToken.None, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = await FindAsync(cancellationToken, keyValues);

            if (entity == null)
            {
                return false;
            }

            entity.ObjectState = ObjectState.Deleted;
            DbSet.Attach(entity);
            DbSet.Remove(entity);

            return true;
        }

        public virtual void Insert(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Added;
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Add(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entity)
        {

            DbSet.AddRange(entity);
        }

        public virtual RepositoryQuery<TEntity> Query()
        {
            var repositoryGetHelper =
                new RepositoryQuery<TEntity>(this);

            return repositoryGetHelper;
        }

        public IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>>
                includeProperties = null,
            int? page = null,
            int? pageSize = null,
            bool trackChanges = true)
        {
            IQueryable<TEntity> query = DbSet;
            try
            {

                if (trackChanges)
                    query = query.AsNoTracking();

                if (includeProperties != null)
                    includeProperties.ForEach(i => { query = query.Include(i); });

                if (filter != null)
                    query = query.Where(filter);

                if (orderBy != null)
                    query = orderBy(query);

                if (page != null && pageSize != null && orderBy != null)
                    query = query
                        .Skip((page.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value);

            }
            catch (Exception)
            {

                throw;
            }

            return query;
        }

        /// <summary>
        /// </summary>
        /// <param name="sql">string sql</param>
        /// <param name="parameters">params object[] parameters</param>
        /// <returns>IQueryable<TEntity></returns>
        public IQueryable<TEntity> ExecuteSql(string sql, params object[] parameters)
        {
            IQueryable<TEntity> res;
            try
            {
                (Context as DbContext).Database.CommandTimeout = 600;
                res = (Context as DbContext).Database.SqlQuery<TEntity>(sql, parameters).AsQueryable();
            }
            catch (Exception)
            {
                throw;
            }
            return res;
        } // ExecuteSql

        public int ExecuteUpdateSql(string sql, params object[] parameters)
        {
            int count = 0;
            try
            {
                (Context as DbContext).Database.CommandTimeout = 180;
                count = (Context as DbContext).Database.ExecuteSqlCommand(sql, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
            return count;
        } // Ex
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
