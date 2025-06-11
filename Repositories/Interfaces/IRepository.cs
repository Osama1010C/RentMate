using System.Linq.Expressions;

namespace RentMateAPI.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(int? id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(int id);

        Task<bool> IsExistAsync(int id);

        Task<List<T>> Skip(int NumberofSkips);
        Task<List<T>> Take(int NumberofTakes);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                                     Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                     string includeProperties = "");

        Task<T> GetAsync(Expression<Func<T, bool>> filter = null,
                                    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                    string includeProperties = "");

        Task<List<T>> GetAllAsync(int skip, int take, string includeProperties = "");

        Task SaveChangesAsync();
    }
}
