using SimpleFileDbV2.db;

namespace SimpleFileDatabase;

public interface ISimpleFileDb
{
    Task CreateAsync<Tdata>(Tdata data, Transaction? transaction = null) where Tdata : class;
    Task UpdateAsync<Tdata>(Tdata data, Transaction? transaction = null) where Tdata : class;
    Task<Tdata?> GetByIdAsync<Tdata>(object id) where Tdata : class;
    IAsyncEnumerable<Tdata> GetAllAsync<Tdata>() where Tdata : class;
    Task DeleteByIdAsync<Tdata>(object id, Transaction? transaction = null) where Tdata : class;
    Transaction BeginTransaction();
    void CommitTransaction(Transaction transaction);
}
