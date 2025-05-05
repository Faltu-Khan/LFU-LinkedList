using System.Collections.Generic;

namespace LFUCache.Library.Interfaces
{
    public interface ICache<TKey, TValue>
    {
        void Put(TKey key, TValue value);
        TValue Get(TKey key);
        void Update(TKey key, TValue value);
        void Remove(TKey key);
        IEnumerable<(TKey Key, TValue Value, int Frequency, DateTime LastAccess)> GetAll();
    }
}