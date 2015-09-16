using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilyPath.Utility
{
    internal class ZeroList : IList<float>
    {
        public int IndexOf (float item)
        {
            return 0;
        }

        public void Insert (int index, float item)
        { }

        public void RemoveAt (int index)
        { }

        public float this[int index]
        {
            get { return 0; }
            set { }
        }

        public void Add (float item)
        { }

        public void Clear ()
        { }

        public bool Contains (float item)
        {
            return false;
        }

        public void CopyTo (float[] array, int arrayIndex)
        { }

        public int Count
        {
            get { return 0; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove (float item)
        {
            return false;
        }

        public IEnumerator<float> GetEnumerator ()
        {
            yield break;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            yield break;
        }
    }
}
