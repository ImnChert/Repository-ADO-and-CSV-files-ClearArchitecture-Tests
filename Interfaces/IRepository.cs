using Core;

namespace Interfaces
{
    public interface IRepository<T> where T : Entity
    {
        public Task CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
        public Task<T> GetAsync(int id);
        public Task<List<T>> GetAllAsync();
    }
}
