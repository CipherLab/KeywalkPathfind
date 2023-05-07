using System.Collections.Concurrent;

namespace KeyWalkAnalyzer
{
    public class ConcurrentHashSet<T>
    {
        private readonly ConcurrentDictionary<T, byte> _dictionary;
        private const byte DummyValue = 0;

        public ConcurrentHashSet()
        {
            _dictionary = new ConcurrentDictionary<T, byte>();
        }

        public bool TryAdd(T item)
        {
            return _dictionary.TryAdd(item, DummyValue);
        }

        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        public bool TryRemove(T item)
        {
            return _dictionary.TryRemove(item, out _);
        }
    }
}