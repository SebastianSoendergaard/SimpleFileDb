namespace SimpleFileDbV1.db
{
    internal interface ISimpleFileDb
    {
        public void Save<Tdata>(Tdata data) where Tdata : class;
        public Tdata? GetById<Tdata>(object id) where Tdata : class;
        public IEnumerable<Tdata> GetAll<Tdata>() where Tdata : class;
        public void DeleteById<Tdata>(object id) where Tdata : class;
    }
}
