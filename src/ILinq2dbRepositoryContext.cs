using LinqToDB.Data;

namespace TableRepo.linq2db
{
    public interface ILinq2dbRepositoryContext
    {
        DataConnection GetDataContext();
        Task<DataConnectionTransaction> StartTransactionAsync(CancellationToken cancellation = default);
        Task CommitAsync(CancellationToken cancellation = default);
        Task RollbackAsync(CancellationToken cancellation = default);
        Task CloseConnectionAsync();
    }
}