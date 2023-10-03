using LinqToDB;
using System.Linq.Expressions;

namespace TableRepo.linq2db
{
    public interface ILinq2dbRepository<T> where T : class
    {
        ITable<T> Table { get; }
        Task<int> InsertAsync(T entity, CancellationToken cancellation = default);
        Task<long> BulkInsertAsync(List<T> entities, CancellationToken cancellation = default);
        Task<int> ModifyAsync(T entity, CancellationToken cancellation = default);
        Task<int> RemoveAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellation = default);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>>? criteria = null, CancellationToken cancellation = default);
        Task<List<T>> FindAllAsync(Func<IQueryable<T>, IQueryable<T>> func, CancellationToken cancellation = default);
        Task<T?> FindFirstAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellation = default);
        Task<T?> FindFirstAsync(Func<IQueryable<T>, IQueryable<T>> func, CancellationToken cancellation = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null, CancellationToken cancellation = default);
    }
}