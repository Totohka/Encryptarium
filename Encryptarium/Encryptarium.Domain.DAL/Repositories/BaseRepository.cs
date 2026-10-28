using Encryptarium.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Encryptarium.Domain.DAL.Repositories
{
    public interface IBaseRepository<T> where T : struct
    {
        Task<Y> GetAsync<Y>(T id) where Y : BaseEntity<T>;
        Task<Y> GetAsync<Y>(Expression<Func<Y, bool>> predicate) where Y : BaseEntity<T>;
        Task<T> CreateAsync<Y>(Y entity) where Y : BaseEntity<T>;
        Task UpdateAsync<Y>(Y entity) where Y : BaseEntity<T>;
        Task DeleteAsync<Y>(T id) where Y : BaseEntity<T>;
        Task DeleteAsync<Y>(Y entity) where Y : BaseEntity<T>;
        IQueryable<Y> GetAll<Y>() where Y : BaseEntity<T>;
        IQueryable<Y> GetAll<Y>(Expression<Func<Y, bool>> predicate) where Y : BaseEntity<T>;
        Task<List<Y>> GetAllAsync<Y>() where Y : BaseEntity<T>;
        Task<List<Y>> GetAllAsync<Y>(Expression<Func<Y, bool>> predicate) where Y : BaseEntity<T>;
    }

    public class BaseRepository<T> : IBaseRepository<T> where T : struct
    {
        private readonly EncryptariumContext _context;
        public BaseRepository(EncryptariumContext context) 
        {
            _context = context;
        }

        public async Task<T> CreateAsync<Y>(Y entity) where Y : BaseEntity<T>
        {
            await _context.Set<Y>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync<Y>(T id) where Y : BaseEntity<T>
        {
            var entity = await GetAsync<Y>(id);
            await DeleteAsync(entity);
        }

        public async Task DeleteAsync<Y>(Y entity) where Y : BaseEntity<T>
        {
            _context.Set<Y>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Y> GetAll<Y>() where Y : BaseEntity<T>
        {
            return _context.Set<Y>().AsQueryable();
        }

        public IQueryable<Y> GetAll<Y>(Expression<Func<Y, bool>> predicate) where Y : BaseEntity<T>
        {
            return _context.Set<Y>().Where(predicate);
        }

        public async Task<Y> GetAsync<Y>(T id) where Y : BaseEntity<T>
        {
            return await _context.Set<Y>().FirstOrDefaultAsync(e => e.Id.Equals(id));
        }

        public async Task UpdateAsync<Y>(Y entity) where Y : BaseEntity<T>
        {
            _context.Set<Y>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Y>> GetAllAsync<Y>() where Y : BaseEntity<T>
        {
            return await _context.Set<Y>().ToListAsync();
        }

        public async Task<List<Y>> GetAllAsync<Y>(Expression<Func<Y, bool>> predicate) where Y : BaseEntity<T>
        {
            return await _context.Set<Y>().Where(predicate).ToListAsync();
        }

        public async Task<Y> GetAsync<Y>(Expression<Func<Y, bool>> predicate) where Y : BaseEntity<T>
        {
            return await _context.Set<Y>().FirstOrDefaultAsync(predicate);
        }
    }
}
