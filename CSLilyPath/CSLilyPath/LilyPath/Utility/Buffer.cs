using System;

namespace LilyPath.Utility
{
    internal class Buffer<T> : IPoolable
    {
        private T[] _buffer;
        private int _index;

        public Buffer ()
            : this(0)
        { }

        public Buffer (int initialCapacity)
        {
            _buffer = new T[initialCapacity];
        }

        public Buffer (T[] data)
        {
            _buffer = data;
        }

        public T this[int index]
        {
            get { return _buffer[index]; }
            set { _buffer[index] = value; }
        }

        public T[] Data 
        {
            get { return _buffer; }
        }

        public int Capacity
        {
            get { return _buffer.Length; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public void SetNext (T value)
        {
            _buffer[_index++] = value;
        }

        public void EnsureCapacity (int capacity)
        {
            if (capacity > _buffer.Length) {
                capacity = 1 << (int)Math.Ceiling(Math.Log(capacity, 2));

                T[] newBuffer = new T[capacity];
                Array.Copy(_buffer, newBuffer, _buffer.Length);
                _buffer = newBuffer;
            }
        }

        public void Reset ()
        {
            _index = 0;
        }
    }

    internal class MicroBuffer<T> : Buffer<T>
    {
        public MicroBuffer ()
            : base()
        { }

        public MicroBuffer (int initialCapacity)
            : base(initialCapacity)
        { }

        public MicroBuffer (T[] data)
            : base(data)
        { }
    }

    /*internal class Buffer2<T> : IList<T>
    {
        private T[] _buffer;
        private int _count;

        public Buffer ()
            : this(0)
        { }

        public Buffer (int initialCapacity)
        {
            _buffer = new T[initialCapacity];
        }

        public int IndexOf (T item)
        {
            for (int i = 0; i < _count; i++) {
                if (item.Equals(_buffer[i]))
                    return i;
            }

            return -1;
        }

        public void Insert (int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt (int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add (T item)
        {
            throw new NotImplementedException();
        }

        public void Clear ()
        {
            throw new NotImplementedException();
        }

        public bool Contains (T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo (T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove (T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator ()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            throw new NotImplementedException();
        }

        private void Resize ()
        {
            Resize(Math.Max(2, _buffer.Length) * 2);
        }

        private void Resize (int newCapacity)
        {
            T[] newBuffer = new T[newCapacity];
            Array.Copy(_buffer, newBuffer, _buffer.Length);
            _buffer = newBuffer;
        }
    }*/
}
