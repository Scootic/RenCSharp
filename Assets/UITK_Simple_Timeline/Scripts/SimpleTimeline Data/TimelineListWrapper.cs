using System;
using System.Collections.Generic;
using System.Collections;

namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Only exists as a wrapper for Unity's Evile Serialization
    /// </summary>
    /// <typeparam name="T">notnull type :(</typeparam>
#nullable enable
#pragma warning disable 
    [Serializable]
    public struct TimelineListWrapper<T> : IList<T> where T : class?
    {
        public List<T?> List;

        public TimelineListWrapper(int length)
        {
            List = new List<T?>();
            while (List.Count < length)
            {
                List.Add(null);
            }
        }

        public readonly T this[int index]
        {
            get
            {
                return List[index];
            }
            set { List[index] = value; }
        }

        public readonly void Sort()
        {
            List.Sort();
        }

        public readonly void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public readonly IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public readonly int Count => List.Count;

        public readonly void Insert(int i, T item)
        {
            List.Insert(i, item);
        }

        public readonly void Clear() { List.Clear(); }

        public readonly bool Contains(T item) { return List.Contains(item); }

        public readonly bool Remove(T item)
        {
            if (Contains(item)) { List.Remove(item); return true; }
            return false;
        }

        public readonly bool IsReadOnly => false;

        public readonly void RemoveAt(int i)
        {
            List.RemoveAt(i);
        }

        public readonly int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public readonly void Add(T item)
        {
            List.Add(item);
        }
    }
#nullable disable
#pragma warning enable
}
