using WebPanel.DAL.Interfaces;
using WebPanel.Domain.Entity;

namespace WebPanel.DAL.Repositories
{
    public class AccountRepository : IBaseRepository<Account>
    {
        private readonly ApplicationDbContext _db;

        public AccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Add(Account entity)
        {
            await _db.Accounts.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Account> GetAll()
        {
            return _db.Accounts;
        }

        public async Task Delete(Account entity)
        {
            _db.Accounts.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRange(List<Account> entity)
        {
            _db.Accounts.RemoveRange(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<Account> Update(Account entity)
        {
            _db.Accounts.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
