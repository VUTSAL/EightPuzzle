using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;


namespace EightPuzzle
{
    
        internal sealed class HeapCollectionDebugView<T>
        {
            private readonly PriorityQueue<T> _Heap;

            public HeapCollectionDebugView(PriorityQueue<T> heap)
            {
                if (ReferenceEquals(null, heap))
                    throw new ArgumentNullException("heap");

                _Heap = heap;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public T[] Items
            {
                get
                {
                    var items = new T[_Heap.Count];
                    _Heap.CopyTo(items, 0);
                    return items;
                }
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        [DebuggerTypeProxy(typeof(HeapCollectionDebugView<>))]
        public sealed class PriorityQueue<T> : ICollection<T>, ICloneable
        {
            private readonly IComparer<T> _Comparer;
            private readonly bool _Reverse;
            private T[] _Items;
            private int _Count;
            private int _Version;

            public static readonly Version Version = new Version(1, 0, 0, 0);

            public const int DefaultSize = 10;

    
            public PriorityQueue(int capacity, IComparer<T> comparer, bool reverse)
            {
                if (capacity <= 0)
                    throw new ArgumentOutOfRangeException("capacity", capacity, "Invalid capacity for heap, must be at least 0");
                if (comparer == null)
                    throw new ArgumentNullException("comparer");

                _Items = new T[Math.Max(capacity, 10)];
                _Comparer = comparer;
                _Reverse = reverse;
            }

    
            public PriorityQueue(int capacity)
                : this(capacity, Comparer<T>.Default, false)
            {
            }

            public PriorityQueue()
                : this(DefaultSize, Comparer<T>.Default, false)
            {
            }

    
            public PriorityQueue(IEnumerable<T> collection)
                : this(Comparer<T>.Default, false, (collection ?? new T[0]).ToArray())
            {
            }

    
            public PriorityQueue(IEnumerable<T> collection, bool reverse)
                : this(Comparer<T>.Default, reverse, (collection ?? new T[0]).ToArray())
            {
            }

    
            public PriorityQueue(params T[] values)
                : this(Comparer<T>.Default, false, values)
            {
            }

    
            public PriorityQueue(IComparer<T> comparer)
                : this(comparer, false)
            {
            }

    
            public PriorityQueue(IComparer<T> comparer, IEnumerable<T> collection)
                : this(comparer, false, (collection ?? new T[0]).ToArray())
            {
            }

    
            public PriorityQueue(IComparer<T> comparer, bool reverse, IEnumerable<T> collection)
                : this(comparer, reverse, (collection ?? new T[0]).ToArray())
            {
            }

    
            public PriorityQueue(IComparer<T> comparer, bool reverse, params T[] values)
            {
                if (comparer == null)
                    throw new ArgumentNullException("comparer");
                if (values == null)
                    throw new ArgumentNullException("values");

                _Items = new T[Math.Max(DefaultSize, values.Length)];
                values.CopyTo(_Items, 0);
                _Count = values.Length;
                _Comparer = comparer;
                _Reverse = reverse;

                for (int index = _Count / 2; index >= 0; index--)
                    SiftUp(index);
            }

    
            public IEnumerator<T> GetEnumerator()
            {
                int version = _Version;
                for (int index = 0; index < _Count; index++)
                {
                    if (version != _Version)
                        throw new InvalidOperationException("Heap has changed, cannot continue iterating over it");

                    yield return _Items[index];
                }
            }

    
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

    
            public void AddRange(IEnumerable<T> collection)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");

                foreach (var element in collection)
                    Add(element);
            }

    
            public void Add(T item)
            {
                if (_Count == _Items.Length)
                    Capacity *= 2;

                _Items[_Count++] = item;
                SiftDown(0, _Count - 1);
                BreakEnumerators();
            }

    
            private void SiftDown(int startPos, int pos)
            {
                T newItem = _Items[pos];
                while (pos > startPos)
                {
                    int parentPos = (pos - 1) / 2;
                    T parent = _Items[parentPos];
                    if (CompareElements(parent, newItem) <= 0)
                        break;
                    _Items[pos] = parent;
                    pos = parentPos;
                }

                _Items[pos] = newItem;
                BreakEnumerators();
            }

    
            private void SiftUp(int pos)
            {
                int endPos = _Count;
                int startPos = pos;
                T newItem = _Items[pos];

                int childPos = (2 * pos) + 1;

                while (childPos < endPos)
                {
                    int rightPos = childPos + 1;
                    if (rightPos < endPos && CompareElements(_Items[rightPos], _Items[childPos]) <= 0)
                        childPos = rightPos;

                    _Items[pos] = _Items[childPos];
                    pos = childPos;
                    childPos = (2 * pos) + 1;
                }

                _Items[pos] = newItem;
                SiftDown(startPos, pos);
                BreakEnumerators();
            }

    
            private void BreakEnumerators()
            {
                unchecked
                {
                    _Version++;
                }
            }

           
            public void TrimExcess()
            {
                if (_Count < _Items.Length * 0.9)
                    Capacity = _Count;
            }

           
            public void Push(T value)
            {
                Add(value);
            }

           
            public T Pop()
            {
                if (_Count == 0)
                    throw new InvalidOperationException("Cannot pop from the heap, it is empty");

                T lastElement = _Items[--_Count];
                _Items[_Count] = default(T);
                T returnItem;
                if (_Count > 0)
                {
                    returnItem = _Items[0];
                    _Items[0] = lastElement;
                    SiftUp(0);
                }
                else
                    returnItem = lastElement;

                BreakEnumerators();

                return returnItem;
            }

           
            public T ReplaceAt(int index, T newValue)
            {
                if (index < 0 || index >= _Count)
                    throw new ArgumentOutOfRangeException("index");

                T returnElement = _Items[index];
                if (index == 0)
                {
                    _Items[0] = newValue;
                    SiftUp(0);

                    BreakEnumerators();
                }
                else
                {
                    RemoveAt(index);
                    Add(newValue);
                }

                return returnElement;
            }

           
            public void RemoveAt(int index)
            {
                if (index < 0 || index >= _Count)
                    throw new ArgumentOutOfRangeException("index");

                if (index < _Count - 1)
                    _Items[index] = _Items[_Count - 1];

                _Count--;
                _Items[_Count] = default(T);

                if (index < _Count)
                    SiftUp(index);

                BreakEnumerators();
            }

            
            public bool Replace(T value, T newValue)
            {
                int index = IndexOf(value);
                if (index < 0)
                    return false;

                ReplaceAt(index, newValue);
                return true;
            }

           
            private int CompareElements(T element1, T element2)
            {
                int result = _Comparer.Compare(element1, element2);
                if (_Reverse)
                    result = -result;
                return result;
            }

          
            public void Clear()
            {
                for (int index = 0; index < _Count; index++)
                    _Items[index] = default(T);
                _Count = 0;
                BreakEnumerators();
            }

          
            public bool Contains(T item)
            {
                return IndexOf(item) >= 0;
            }

            
            public int IndexOf(T value)
            {
                for (int index = 0; index < _Count; index++)
                    if (CompareElements(value, _Items[index]) == 0)
                        return index;

                return -1;
            }

           
            public int ParentIndex(int index)
            {
                if (index < 0 || index >= _Count)
                    throw new ArgumentOutOfRangeException("index");

                if (_Count == 0)
                    return -1;
                if (index == 0)
                    return -1;
                return (index - 1) / 2;
            }

           
            public int LeftChildIndex(int index)
            {
                if (index < 0 || index >= _Count)
                    throw new ArgumentOutOfRangeException("index");

                index = (index * 2) + 1;
                if (index >= _Count)
                    return -1;
                return index;
            }

         
            public int RightChildIndex(int index)
            {
                if (index < 0 || index >= _Count)
                    throw new ArgumentOutOfRangeException("index");

                index = (index * 2) + 2;
                if (index >= _Count)
                    return -1;
                return index;
            }

            
            public void CopyTo(T[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException("array");
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "arrayIndex cannot be negative");
                if (arrayIndex + Count > array.Length)
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The number of elements in the heap ({0}) is greater than the available space in the array ({1}) from the given arrayIndex ({2})", Count, array.Length, arrayIndex));

                for (int index = 0; index < _Count; index++)
                    array[arrayIndex + index] = _Items[index];
            }

           
            public bool Remove(T item)
            {
                if (Count == 0)
                    return false;

                int index = IndexOf(item);
                if (index < 0)
                    return false;

                RemoveAt(index);

                return true;
            }

           
            public int Count
            {
                get
                {
                    return _Count;
                }
            }

          
            public IComparer<T> Comparer
            {
                get
                {
                    return _Comparer;
                }
            }

            public bool Reverse
            {
                get
                {
                    return _Reverse;
                }
            }

           
            public T this[int index]
            {
                get
                {
                    if (index < 0 || index >= _Count)
                        throw new ArgumentOutOfRangeException("index");
                    return _Items[index];
                }
            }

            
            public int Capacity
            {
                get
                {
                    return _Items.Length;
                }

                set
                {
                    if (value < _Count)
                        throw new ArgumentOutOfRangeException("value", value, "Heap capacity cannot be that small (" + value + "), must at least be equal to Count (" + Count + ")");

                    int newSize = Math.Max(value, DefaultSize);
                    if (newSize != _Items.Length)
                    {
                        var newElements = new T[newSize];
                        if (_Items.Length > 0)
                            Array.Copy(_Items, 0, newElements, 0, Math.Min(_Items.Length, value));
                        _Items = newElements;
                        unchecked
                        {
                            _Version++;
                        }
                    }
                }
            }

          
            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public PriorityQueue<T> Clone()
            {
                var result = new PriorityQueue<T>(_Comparer, _Reverse);
                result.Capacity = _Items.Length;
                result._Count = _Count;
                for (int index = 0; index < _Count; index++)
                    result._Items[index] = _Items[index];

                result._Version = _Version;
                return result;
            }

            object ICloneable.Clone()
            {
                return Clone();
            }
        }
    
}
