using LFUCache.Library.Interfaces;
using LFUCache.Library.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace LFUCache.Library.Services
{
    public class LFUCache : ICache<int, string>
    {
        private readonly int capacity;
        private int keyCounter = 1;

        private readonly Dictionary<int, (string Value, int Frequency, DateTime LastAccess)> cache = new();
        private readonly Dictionary<int, LinkedList<int>> freqMap = new();
        private readonly Dictionary<int, LinkedListNode<int>> nodeMap = new();
        private readonly object lockObject = new();

        public LFUCache(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException("Cache capacity must be > 0");
            this.capacity = capacity;
        }

        public void Put(int key, string value)
        {
            lock (lockObject)
            {
                if (cache.ContainsKey(key))
                    throw new DuplicateItemException("Key already exists.");

                if (cache.Count >= capacity)
                    EvictLFU();

                cache[key] = (value, 1, DateTime.UtcNow);

                if (!freqMap.ContainsKey(1))
                    freqMap[1] = new LinkedList<int>();
                nodeMap[key] = freqMap[1].AddLast(key);
            }
        }

        public string Get(int key)
        {
            lock (lockObject)
            {
                if (!cache.ContainsKey(key))
                    throw new ItemNotFoundException("Key not found.");

                UpdateFrequency(key);
                return cache[key].Value;
            }
        }

        public void Update(int key, string value)
        {
            lock (lockObject)
            {
                if (!cache.ContainsKey(key))
                    throw new ItemNotFoundException("Key not found.");

                cache[key] = (value, cache[key].Frequency, DateTime.UtcNow);
                UpdateFrequency(key);
            }
        }

        public void Remove(int key)
        {
            lock (lockObject)
            {
                if (!cache.ContainsKey(key))
                    throw new ItemNotFoundException("Key not found.");

                int freq = cache[key].Frequency;
                freqMap[freq].Remove(nodeMap[key]);
                if (freqMap[freq].Count == 0)
                    freqMap.Remove(freq);

                nodeMap.Remove(key);
                cache.Remove(key);
            }
        }

        public IEnumerable<(int Key, string Value, int Frequency, DateTime LastAccess)> GetAll()
        {
            lock (lockObject)
            {
                return cache.Select(kv => (kv.Key, kv.Value.Value, kv.Value.Frequency, kv.Value.LastAccess)).ToList();
            }
        }

        private void UpdateFrequency(int key)
        {
            var (val, freq, _) = cache[key];
            freqMap[freq].Remove(nodeMap[key]);
            if (freqMap[freq].Count == 0)
                freqMap.Remove(freq);

            int newFreq = freq + 1;
            if (!freqMap.ContainsKey(newFreq))
                freqMap[newFreq] = new LinkedList<int>();

            nodeMap[key] = freqMap[newFreq].AddLast(key);
            cache[key] = (val, newFreq, DateTime.UtcNow);
        }

        private void EvictLFU()
        {
            var minFreq = freqMap.Keys.Min();
            var keyToRemove = freqMap[minFreq].First.Value;

            freqMap[minFreq].RemoveFirst();
            if (freqMap[minFreq].Count == 0)
                freqMap.Remove(minFreq);

            cache.Remove(keyToRemove);
            nodeMap.Remove(keyToRemove);
        }
    }
}