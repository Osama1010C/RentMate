using Microsoft.EntityFrameworkCore;
using RentMateAPI.Data;
using RentMateAPI.Repositories.Interfaces;
using System.Linq.Expressions;

namespace RentMateAPI.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        private readonly DbSet<T> _dbSet;
        public Repository(AppDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<T?> GetByIdAsync(int? id) => await _dbSet.FindAsync(id);
        

        public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();
        
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);
        
        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
                _dbSet.Remove(entity);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                                                  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                  string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(int skip, int take, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet.AsNoTracking().Skip(skip).Take(take);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            

            return await query.ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(int skip, int take, string includeProperties = "", Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            if (filter != null)
                query = query.Where(filter);

            query = query.Skip(skip).Take(take);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);



            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null,
                                 Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                 string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null)
                return await orderBy(query).FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

        public async Task<bool> IsExistAsync(int id) => await _dbSet.FindAsync(id) != null;


    }
}
