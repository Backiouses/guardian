using System;
using System.Collections;
using System.Collections.Generic;

internal class SynchronizedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private List<T> RealList = new List<T>();

	private object AccessLock = new object();

	public T this[int index]
	{
		get
		{
			lock (AccessLock)
			{
				return RealList[index];
			}
		}
		set
		{
			lock (AccessLock)
			{
				RealList[index] = value;
			}
		}
	}

	public int Count
	{
		get
		{
			lock (AccessLock)
			{
				return RealList.Count;
			}
		}
	}

	public bool IsReadOnly => ((ICollection<T>)RealList).IsReadOnly;

	public void Add(T item)
	{
		lock (AccessLock)
		{
			RealList.Add(item);
		}
	}

	public void Clear()
	{
		lock (AccessLock)
		{
			RealList.Clear();
		}
	}

	public bool Contains(T item)
	{
		lock (AccessLock)
		{
			return RealList.Contains(item);
		}
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		lock (AccessLock)
		{
			RealList.CopyTo(array, arrayIndex);
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		lock (AccessLock)
		{
			return RealList.GetEnumerator();
		}
	}

	public int IndexOf(T item)
	{
		lock (AccessLock)
		{
			return RealList.IndexOf(item);
		}
	}

	public void Insert(int index, T item)
	{
		lock (AccessLock)
		{
			RealList.Insert(index, item);
		}
	}

	public bool Remove(T item)
	{
		lock (AccessLock)
		{
			return RealList.Remove(item);
		}
	}

	public void RemoveAt(int index)
	{
		lock (AccessLock)
		{
			RealList.RemoveAt(index);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		lock (AccessLock)
		{
			return RealList.GetEnumerator();
		}
	}

	public void RemoveAll(Predicate<T> predicate)
	{
		lock (AccessLock)
		{
			RealList.RemoveAll(predicate);
		}
	}
}
