using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Controls.Utils;

internal static class CollectionUtils
{
	private class FastRepeat<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public static readonly FastRepeat<T> Instance = new FastRepeat<T>();

		public int Count { get; set; }

		public bool IsReadOnly => true;

		public T Item
		{
			get; [param: AllowNull]
			set;
		}

		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			int num = arrayIndex + Count;
			for (int i = arrayIndex; i < num; i++)
			{
				array[i] = Item;
			}
		}
	}

	public static NotifyCollectionChangedEventArgs ResetEventArgs { get; } = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

	public static void InsertMany<T>(this List<T> list, int index, T item, int count)
	{
		FastRepeat<T> instance = FastRepeat<T>.Instance;
		instance.Count = count;
		instance.Item = item;
		list.InsertRange(index, FastRepeat<T>.Instance);
		instance.Item = default(T);
	}
}
