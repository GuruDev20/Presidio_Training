using System.Linq.Expressions;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        public Task<T?> GetByIdAsync(Guid id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        public Task AddAsync(T entity);
        public Task AddRangeAsync(IEnumerable<T> entities);
        public void Update(T entity);
        public void Remove(T entity);
        public void RemoveRange(IEnumerable<T> entities);
        public Task<bool> SaveChangesAsync();
    }
}