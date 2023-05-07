using System.Collections.Generic;
using System.Threading;

namespace KeyWalkAnalyzer
{
    public class FastConcurrentHashSet<T>
    {
        private readonly HashSet<T> _hashSet;
        private readonly ReaderWriterLockSlim _lock;

        public FastConcurrentHashSet()
        {
            _hashSet = new HashSet<T>();
            _lock = new ReaderWriterLockSlim();
        }

        public bool TryAdd(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _hashSet.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _hashSet.Contains(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool TryRemove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _hashSet.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}