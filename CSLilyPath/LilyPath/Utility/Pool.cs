using System;
using System.Collections.Generic;

namespace LilyPath.Utility
{
    internal interface IPoolable
    {
        void Reset ();
    }

    internal abstract class Pool
    {
        public abstract int MaxReserve { get; set; }
        public abstract int Peak { get; set; }
        public abstract int Count { get; }

        public void Release (object obj)
        {
            ReleaseCore(obj);
        }

        protected abstract void ReleaseCore (object obj);

        public abstract void Clear ();
    }

    internal sealed class Pool<T> : Pool
        where T : new()
    {
        private Stack<T> _free;

        public Pool ()
            : this(16, int.MaxValue)
        { }

        public Pool (int initialCapacity)
            : this(initialCapacity, int.MaxValue)
        { }

        public Pool (int initialCapacity, int max)
        {
            MaxReserve = max;
            _free = new Stack<T>(initialCapacity);
        }

        public override int MaxReserve { get; set; }

        public override int Peak { get; set; }

        public override int Count
        {
            get { return _free.Count; }
        }

        public T Obtain ()
        {
            if (_free.Count == 0)
                return new T();

            return _free.Pop();
        }

        protected override void ReleaseCore (object obj)
        {
            if (obj is T)
                Release((T)obj);
        }

        public void Release (T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (_free.Count < MaxReserve) {
                _free.Push(obj);
                Peak = Math.Max(Peak, _free.Count);
            }

            if (obj is IPoolable)
                (obj as IPoolable).Reset();
        }

        public void Release (IList<T> objects)
        {
            if (objects == null)
                throw new ArgumentNullException("objects");

            foreach (T obj in objects) {
                if (obj == null)
                    continue;

                if (_free.Count < MaxReserve)
                    _free.Push(obj);

                if (obj is IPoolable)
                    (obj as IPoolable).Reset();
            }

            Peak = Math.Max(Peak, _free.Count);
        }

        public override void Clear ()
        {
            _free.Clear();
            Peak = 0;
        }
    }
}
