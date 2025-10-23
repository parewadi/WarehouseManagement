using System.Linq.Expressions;

namespace WarehouseManagement.API.Models.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQueryable();
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetAsync(
           Expression<Func<T, bool>> filter,
           string? includeProperties = null
       );

        Task<IEnumerable<T>> GetWithIncludesAsync( Expression<Func<T, bool>>? filter = null, string includeProperties = "");
    }
}
