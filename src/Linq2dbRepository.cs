using LinqToDB;
using LinqToDB.Data;
using System.Linq.Expressions;

namespace TableRepo.linq2db
{
    public class Linq2dbRepository<T> : ILinq2dbRepository<T> where T : class
    {
        private readonly ILinq2dbRepositoryContext _context;

        public Linq2dbRepository(ILinq2dbRepositoryContext context)
        {
            _context = context;
        }

        public ITable<T> Table
        {
            get
            {
                return GetTable();
            }
        }

        public DataConnection GetDataConnection()
        {
            return _context.GetDataContext();
        }

        private ITable<T> GetTable()
        {
            return GetDataConnection().GetTable<T>();
        }

        public async Task<int> InsertAsync(T entity,
            CancellationToken cancellation = default)
        {
            return await Table.AddAsync(entity, cancellation);
        }

        public async Task<long> BulkInsertAsync(List<T> entities,
            CancellationToken cancellation = default)
        {
            var bulkOperation = await Table.BulkCopyAsync(entities, cancellation);

            return bulkOperation.RowsCopied;
        }

        public async Task<int> ModifyAsync(T entity,
            CancellationToken cancellation = default)
        {
            return await Table.ModifyAsync(entity, cancellation);
        }

        public async Task<int> RemoveAsync(Expression<Func<T, bool>> criteria,
            CancellationToken cancellation = default)
        {
            return await Table.Where(criteria).DeleteAsync(cancellation);
        }

        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>>? criteria = null,
            CancellationToken cancellation = default)
        {
            var query = Table.AsQueryable();

            if (criteria != null)
                query = query.Where(criteria);

            //return await query.ToListAsync(cancellation);

            var response = await query.ToListAsync(cancellation);

            await _context.CloseConnectionAsync();

            return response;
        }

        public async Task<T?> FindFirstAsync(Expression<Func<T, bool>> criteria,
            CancellationToken cancellation = default)
        {
            if (criteria == null)
                return default(T);

            var query = Table
                .Where(criteria);

            var response = await query.FirstOrDefaultAsync(cancellation);

            await _context.CloseConnectionAsync();

            return response;
        }

        public async Task<List<T>> FindAllAsync(Func<IQueryable<T>, IQueryable<T>> func,
            CancellationToken cancellation = default)
        {
            var query = Table.AsQueryable();

            query = func != null ? func(query) : query;

            //return await query.ToListAsync(cancellation);

            var response = await query.ToListAsync(cancellation);

            await _context.CloseConnectionAsync();

            return response;
        }

        public async Task<T?> FindFirstAsync(Func<IQueryable<T>, IQueryable<T>> func,
            CancellationToken cancellation = default)
        {
            var query = Table.AsQueryable();

            query = func != null ? func(query) : query;

            var response = await query.FirstOrDefaultAsync(cancellation);

            await _context.CloseConnectionAsync();

            return response;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null,
            CancellationToken cancellation = default)
        {
            var query = Table.AsQueryable();

            if (criteria != null)
                query = query.Where(criteria);

            var response = await query.CountAsync(cancellation);

            await _context.CloseConnectionAsync();

            return response;
        }
    }
}