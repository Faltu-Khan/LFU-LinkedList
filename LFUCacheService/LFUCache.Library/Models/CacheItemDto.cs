using System;

namespace LFUCache.Library.Models
{
    public class CacheItemDto
    {
        public int Key { get; set; }
        public string Value { get; set; }
        public int Frequency { get; set; }
        public DateTime LastAccess { get; set; }
    }
}