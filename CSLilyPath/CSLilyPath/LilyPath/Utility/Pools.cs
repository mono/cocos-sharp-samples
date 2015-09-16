using System.Collections.Generic;

namespace LilyPath.Utility
{
    internal static class Pools<T>
        where T : new()
    {
        private static readonly Pool<T> _pool = new Pool<T>();

        public static Pool<T> Pool
        {
            get { return _pool; }
        }

        public static T Obtain ()
        {
            lock (_pool) {
                return _pool.Obtain();
            }
        }

        public static void Release (T obj)
        {
            lock (_pool) {
                _pool.Release(obj);
            }
        }

        public static void Release (IList<T> objects)
        {
            lock (_pool) {
                _pool.Release(objects);
            }
        }
    }
}
