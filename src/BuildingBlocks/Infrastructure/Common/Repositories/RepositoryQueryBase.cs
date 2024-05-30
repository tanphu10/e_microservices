using Contracts.Common.Interfaces;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Common.Repositories
{
    public class RepositoryQueryBase<T, K>
    where T : EntityBase<K>
    {

    }
    public class RepositoryQueryBase<T, K, TContext> : RepositoryQueryBase<T, K>, IRepositoryQueryBase<T, K, TContext>
     where T : EntityBase<K>
     where TContext : DbContext
    {
        private readonly TContext _dbContext;

        public RepositoryQueryBase(TContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IQueryable<T> FindAll(bool trackChanges = false)
        {

            var res = !trackChanges ? _dbContext.Set<T>().AsNoTracking() :
                   _dbContext.Set<T>();
            return res;
        }
        public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var items = FindAll(trackChanges);
            items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
            return items;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
        {
            if (!trackChanges)
            {
                var result = _dbContext.Set<T>().Where(expression).AsNoTracking();
                return result;
            }
            else
            {
                var result = _dbContext.Set<T>().Where(expression);
                return result;
            }
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var items = FindByCondition(expression, trackChanges);
            items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
            return items;
        }

        public async Task<T?> GetByIdAsync(K id)
        {
            var res = await FindByCondition(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();
            return res;
        }

        public async Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties) =>
    await FindByCondition(x => x.Id.Equals(id), trackChanges: false, includeProperties)
        .FirstOrDefaultAsync();
    }
}
