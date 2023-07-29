using System.Text.Json;

namespace SimpleFileDbV1.db
{
    internal class SimpleFileDb : ISimpleFileDb
    {
        private readonly SimpleFileDbConfiguration _configuration;

        public SimpleFileDb(SimpleFileDbConfiguration configuration)
        {
            _configuration = configuration;

            Directory.CreateDirectory(_configuration.DirectoryPath);
        }

        public void Create<Tdata>(Tdata data) where Tdata : class
        {
            var id = GetIdFromData(data);
            var path = CreateFilePath<Tdata>(id);
            if (File.Exists(path))
            {
                throw new AlreadyExistException($"Id for {typeof(Tdata).FullName} already exist");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(path, json);
        }

        public void Update<Tdata>(Tdata data) where Tdata : class
        {
            var id = GetIdFromData(data);
            var path = CreateFilePath<Tdata>(id);
            if (!File.Exists(path))
            {
                throw new NotFoundException($"Id for {typeof(Tdata).FullName} not found");
            }

            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(path, json);
        }

        public Tdata? GetById<Tdata>(object id) where Tdata : class
        {
            var path = CreateFilePath<Tdata>(id);
            if (!File.Exists(path))
            {
                return null;
            }

            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<Tdata>(json);
            return data;
        }

        public IEnumerable<Tdata> GetAll<Tdata>() where Tdata : class
        {
            var path = CreateDirectoryPath<Tdata>();
            var files = Directory.EnumerateFiles(path);
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var data = JsonSerializer.Deserialize<Tdata>(json);
                if (data != null)
                {
                    yield return data;
                }
            }
        }

        public void DeleteById<Tdata>(object id) where Tdata : class
        {
            var path = CreateFilePath<Tdata>(id);
            File.Delete(path);
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
    }
}
