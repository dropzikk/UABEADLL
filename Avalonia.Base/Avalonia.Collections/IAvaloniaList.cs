using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Avalonia.Collections;

public interface IAvaloniaList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IAvaloniaReadOnlyList<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
	new int Count { get; }

	new T this[int index] { get; set; }

	void AddRange(IEnumerable<T> items);

	void InsertRange(int index, IEnumerable<T> items);

	void Move(int oldIndex, int newIndex);

	void MoveRange(int oldIndex, int count, int newIndex);

	void RemoveAll(IEnumerable<T> items);

	void RemoveRange(int index, int count);
}
