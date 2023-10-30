namespace SimpleFileDbV2.db
{
    public class Transaction : IDisposable
    {
        private readonly Action<Transaction> _onDispose;

        public Transaction(Action<Transaction> onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose(this);
        }
    }
}
