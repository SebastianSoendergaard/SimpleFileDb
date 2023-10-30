namespace SimpleFileDbV2.db
{
    internal interface IDataChangeEvent { }

    internal record DataCreateEvent(string FilePath, string Data) : IDataChangeEvent;

    internal record DataUpdateEvent(string FilePath, string Data) : IDataChangeEvent;

    internal record DataDeleteEvent(string FilePath) : IDataChangeEvent;
}
