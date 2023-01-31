using Core.Models;

namespace Interfaces
{
    public interface IUserRepository<T> : IRepository<T>
        where T : User
    {
        public List<Trophy> GetTrophies(T entity);
    }
}
