using System.Linq.Expressions;

namespace WarehouseManagement.API.Models.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQueryable();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
