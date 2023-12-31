﻿namespace SimpleFileDatabase;

public class SimpleFileDbConfiguration
{
    public SimpleFileDbConfiguration(string directoryPath)
    {
        DirectoryPath = directoryPath;
    }

    public string DirectoryPath { get; }
    public Dictionary<Type, IIdConverter> IdConverters { get; } = new Dictionary<Type, IIdConverter>();
    public SimpleFileDbConfiguration RegisterDataType<Tdata>(Func<Tdata, object> dataTypeToIdValue) where Tdata : class
    {
        var converter = new TypedIdConverter<Tdata>(dataTypeToIdValue);
        IdConverters.Add(typeof(Tdata), converter);
        return this;
    }

    public interface IIdConverter { }

    public class TypedIdConverter<Tdata> : IIdConverter
    {
        private readonly Func<Tdata, object> _converter;

        public TypedIdConverter(Func<Tdata, object> converter)
        {
            _converter = converter;
        }

        public object GetId(Tdata data)
        {
            return _converter(data) ?? throw new ArgumentNullException($"Id for {typeof(Tdata).FullName} cannot be null");
        }
    }

}
