using SimpleFileDbV2.db;
using System.Text.Json;

namespace SimpleFileDatabase;

internal class SimpleFileDb : ISimpleFileDb
{
    private readonly SimpleFileDbConfiguration _configuration;
    private readonly TransactionManager _transactionManager;

    public SimpleFileDb(SimpleFileDbConfiguration configuration)
    {
        _configuration = configuration;
        _transactionManager = new TransactionManager();

        Directory.CreateDirectory(_configuration.DirectoryPath);
    }

    public async Task CreateAsync<Tdata>(Tdata data, Transaction? transaction = null) where Tdata : class
    {
        var id = GetIdFromData(data);
        var path = CreateFilePath<Tdata>(id);

        if (File.Exists(path))
        {
            throw new AlreadyExistException($"Id for {typeof(Tdata).FullName} already exist");
        }

        var json = JsonSerializer.Serialize(data);
        var changeEvent = new DataCreateEvent(path, json);

        if (transaction != null)
        {
            _transactionManager.AddTransactionEvent(transaction, changeEvent);
            await Task.CompletedTask;
        }
        else
        {
            await SaveChanges(new[] { changeEvent });
        }
    }

    public async Task UpdateAsync<Tdata>(Tdata data, Transaction? transaction = null) where Tdata : class
    {
        var id = GetIdFromData(data);
        var path = CreateFilePath<Tdata>(id);

        if (!File.Exists(path))
        {
            throw new NotFoundException($"Id for {typeof(Tdata).FullName} not found");
        }

        var json = JsonSerializer.Serialize(data);
        var changeEvent = new DataUpdateEvent(path, json);

        if (transaction != null)
        {
            _transactionManager.AddTransactionEvent(transaction, changeEvent);
            await Task.CompletedTask;
        }
        else
        {
            await SaveChanges(new[] { changeEvent });
        }
    }

    public async Task<Tdata?> GetByIdAsync<Tdata>(object id) where Tdata : class
    {
        var path = CreateFilePath<Tdata>(id);
        if (!File.Exists(path))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(path);
        var data = JsonSerializer.Deserialize<Tdata>(json);
        return data;
    }

    public async IAsyncEnumerable<Tdata> GetAllAsync<Tdata>() where Tdata : class
    {
        var path = CreateDirectoryPath<Tdata>();
        var files = Directory.EnumerateFiles(path);
        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file);
            var data = JsonSerializer.Deserialize<Tdata>(json);
            if (data != null)
            {
                yield return data;
            }
        }
    }

    public async Task DeleteByIdAsync<Tdata>(object id, Transaction? transaction = null) where Tdata : class
    {
        var path = CreateFilePath<Tdata>(id);
        var changeEvent = new DataDeleteEvent(path);

        if (transaction != null)
        {
            _transactionManager.AddTransactionEvent(transaction, changeEvent);
            await Task.CompletedTask;
        }
        else
        {
            await SaveChanges(new[] { changeEvent });
        }
    }

    private object GetIdFromData<Tdata>(Tdata data) where Tdata : class
    {
        var type = typeof(Tdata);
        if (!_configuration.IdConverters.TryGetValue(type, out var converter))
        {
            throw new TypeNotRegisteredException($"{type.FullName} not registered");
        }

        var typedConverter = converter as SimpleFileDbConfiguration.TypedIdConverter<Tdata>;
        if (typedConverter == null)
        {
            throw new Exception();
        }

        return typedConverter.GetId(data);
    }

    private string CreateFilePath<Tdata>(object id) where Tdata : class
    {
        if (id == null)
        {
            throw new ArgumentNullException($"Id for {typeof(Tdata).FullName} cannot be null");
        }

        var idString = id.ToString();
        if (string.IsNullOrWhiteSpace(idString))
        {
            throw new ArgumentNullException($"Id for {typeof(Tdata).FullName} cannot be empty");
        }

        return Path.Combine(_configuration.DirectoryPath, typeof(Tdata).Name, idString + ".json");
    }

    private string CreateDirectoryPath<Tdata>() where Tdata : class
    {
        return Path.Combine(_configuration.DirectoryPath, typeof(Tdata).Name);
    }

    public Transaction BeginTransaction()
    {
        return _transactionManager.BeginTransaction();
    }

    public void CommitTransaction(Transaction transaction)
    {
        // TODO: 


        // save in transaction file

        // save changes 
    }

    private async Task SaveChanges(IList<IDataChangeEvent> changeEvents)
    {

    }
}
