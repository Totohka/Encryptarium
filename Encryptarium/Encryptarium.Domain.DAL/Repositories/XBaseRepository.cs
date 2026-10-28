using Encryptarium.Domain.Model.Entities;

namespace Encryptarium.Domain.DAL.Repositories
{
    public interface IXBaseRepository
    {
        public IQueryable<Y> GetAll<Y>() where Y : xBaseEntity;
        public Task CreateAsync<Y>(Y entity) where Y : xBaseEntity;
    }

    public class XBaseRepository : IXBaseRepository
    {
        private readonly EncryptariumContext _context;
        public XBaseRepository(EncryptariumContext context)
        {
            _context = context;
        }

        public async Task CreateAsync<Y>(Y entity) where Y : xBaseEntity
        {
            await _context.Set<Y>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Y> GetAll<Y>() where Y : xBaseEntity
        {
            return _context.Set<Y>();
        }
    }
}
