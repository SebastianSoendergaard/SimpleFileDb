namespace SimpleFileDbV1.db
{
    internal class SimpleFileDbException : Exception
    {
        public SimpleFileDbException(string message) : base(message) { }
        public SimpleFileDbException(string message, Exception exception) : base(message, exception) { }
    }

    internal class TypeNotRegisteredException : SimpleFileDbException
    {
        public TypeNotRegisteredException(string message) : base(message) { }
    }

    internal class AlreadyExistException : SimpleFileDbException
    {
        public AlreadyExistException(string message) : base(message) { }
    }

    internal class NotFoundException : SimpleFileDbException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
