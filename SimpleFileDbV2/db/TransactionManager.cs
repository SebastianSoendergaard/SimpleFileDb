using SimpleFileDatabase;
using System.Collections.Concurrent;

namespace SimpleFileDbV2.db
{
    internal class TransactionManager
    {
        private ConcurrentDictionary<Transaction, ConcurrentQueue<IDataChangeEvent>> _transactionEvents = new();

        public void AddTransactionEvent(Transaction transaction, IDataChangeEvent transactionEvent)
        {
            if (!_transactionEvents.ContainsKey(transaction))
            {
                throw new TransactionDisposedException("The transaction has already beeen exposed");
            }

            _transactionEvents[transaction].Enqueue(transactionEvent);
        }

        public Transaction BeginTransaction()
        {
            var transaction = new Transaction(TransactionDisposed);
            _transactionEvents.TryAdd(transaction, new ConcurrentQueue<IDataChangeEvent>());
            return transaction;
        }

        private void TransactionDisposed(Transaction transaction)
        {
            _transactionEvents.TryRemove(transaction, out var _);
        }
    }
}
