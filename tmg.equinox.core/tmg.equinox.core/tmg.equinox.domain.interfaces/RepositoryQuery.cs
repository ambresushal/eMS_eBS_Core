using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;

namespace tmg.equinox.repository.interfaces
{
    public sealed class RepositoryQuery<TEntity> where TEntity : Entity
    {
        private readonly List<Expression<Func<TEntity, object>>>
            _includeProperties;

        private readonly IRepositoryAsync<TEntity> _repository;
        private Expression<Func<TEntity, bool>> _filter;
        private Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> _orderByQuerable;
        private int? _page;
        private int? _pageSize;
        private bool _trackChanges;

        public RepositoryQuery(IRepositoryAsync<TEntity> repository)
        {
            _repository = repository;
            _includeProperties =
                new List<Expression<Func<TEntity, object>>>();
        }

        public RepositoryQuery<TEntity> Filter(
            Expression<Func<TEntity, bool>> filter)
        {
            _filter = filter;
            return this;
        }

        public RepositoryQuery<TEntity> OrderBy(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _orderByQuerable = orderBy;
            return this;
        }

        public RepositoryQuery<TEntity> Include(
            Expression<Func<TEntity, object>> expression)
        {
            _includeProperties.Add(expression);
            return this;
        }

        public RepositoryQuery<TEntity> AsNoTracking(bool trackChanges = true)
        {
            _trackChanges = trackChanges;
            return this;
        }

        public IEnumerable<TEntity> GetPage(
            int page, int pageSize, out int totalCount)
        {
            _page = page;
            _pageSize = pageSize;

            totalCount = _repository.Get(_filter).Count();

            return _repository.Get(
                _filter,
                _orderByQuerable, _includeProperties, _page, _pageSize);
        }

        public IQueryable<TEntity> Get()
        {
            return _repository.Get(
                _filter,
                _orderByQuerable, _includeProperties, _page, _pageSize, _trackChanges);
        }
    }
}
