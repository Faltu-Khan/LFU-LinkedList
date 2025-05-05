using System;

namespace LFUCache.Library.Exceptions
{
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) {}
    }
}