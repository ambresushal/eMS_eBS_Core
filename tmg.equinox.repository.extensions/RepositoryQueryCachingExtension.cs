using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCache;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities;

namespace tmg.equinox.repository.extensions
{

    public static partial class RepositoryQueryExtension
    {
        /// <summary>
        /// This method used to add QueryResult in cache.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repositoryQuery"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> GetCached<TEntity>(this RepositoryQuery<TEntity> repositoryQuery) where TEntity : Entity
        {
            return  repositoryQuery.Get().Cached();
        }
    }
}
