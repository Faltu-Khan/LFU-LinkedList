using System;

namespace LFUCache.Library.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) {}
    }
}