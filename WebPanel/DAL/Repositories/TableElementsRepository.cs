using WebPanel.DAL.Interfaces;
using WebPanel.Domain.Entity;

namespace WebPanel.DAL.Repositories
{
    public class TableElementsRepository : IBaseRepository<TableElement>
    {
        private readonly ApplicationDbContext _db;

        public TableElementsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Add(TableElement entity)
        {
            await _db.TableElements.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<TableElement> GetAll()
        {
            return _db.TableElements;
        }

        public async Task Delete(TableElement entity)
        {
            _db.TableElements.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRange(List<TableElement> entity)
        {
            _db.TableElements.RemoveRange(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<TableElement> Update(TableElement entity)
        {
            _db.TableElements.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
