using WebPanel.DAL.Interfaces;
using WebPanel.Domain.Entity;

namespace WebPanel.DAL.Repositories
{
    public class BaseTableRepository : IBaseRepository<BaseTable>
    {
        private readonly ApplicationDbContext _db;

        public BaseTableRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Add(BaseTable entity)
        {
            await _db.BaseTables.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<BaseTable> GetAll()
        {
            return _db.BaseTables;
        }

        public async Task Delete(BaseTable entity)
        {
            _db.BaseTables.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRange(List<BaseTable> entity)
        {
            _db.BaseTables.RemoveRange(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<BaseTable> Update(BaseTable entity)
        {
            _db.BaseTables.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
