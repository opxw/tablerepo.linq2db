using LinqToDB.Data;

namespace TableRepo.linq2db
{
    public class Linq2dbRepositoryContext : ILinq2dbRepositoryContext
    {
        private readonly IDbConnectionParameter _connectionParameter;
        private DataConnection _db;

        public Linq2dbRepositoryContext(IDbConnectionParameter connectionParameter)
        {
            _connectionParameter = connectionParameter;
            _db = new DataConnection(_connectionParameter.GetProviderName(), _connectionParameter.GetConnectionString());
        }

        public async Task CloseConnectionAsync()
        {
            await _db.CloseAsync();
        }

        public async Task CommitAsync(CancellationToken cancellation = default)
        {
            await _db.CommitTransactionAsync(cancellation);
        }

        public DataConnection GetDataContext()
        {
            return _db;
        }

        public async Task RollbackAsync(CancellationToken cancellation = default)
        {
            await _db.RollbackTransactionAsync(cancellation);
        }

        public async Task<DataConnectionTransaction> StartTransactionAsync(CancellationToken cancellation = default)
        {
            return await _db.BeginTransactionAsync();
        }
    }
}
