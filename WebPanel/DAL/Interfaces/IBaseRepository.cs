namespace WebPanel.DAL.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task Add(T entity);
        Task<T> Update(T entity);
        Task Delete(T entity);
        Task DeleteRange(List<T> entityList);
        IQueryable<T> GetAll();
    }
}
