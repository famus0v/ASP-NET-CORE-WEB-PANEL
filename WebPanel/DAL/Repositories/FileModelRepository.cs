using WebPanel.DAL.Interfaces;
using WebPanel.Domain.Entity;

namespace WebPanel.DAL.Repositories
{
    public class FileModelRepository : IBaseRepository<FileModel>
    {
        private readonly ApplicationDbContext _db;

        public FileModelRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Add(FileModel entity)
        {
            await _db.Files.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<FileModel> GetAll()
        {
            return _db.Files;
        }

        public async Task Delete(FileModel entity)
        {
            _db.Files.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRange(List<FileModel> entity)
        {
            _db.Files.RemoveRange(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<FileModel> Update(FileModel entity)
        {
            _db.Files.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
