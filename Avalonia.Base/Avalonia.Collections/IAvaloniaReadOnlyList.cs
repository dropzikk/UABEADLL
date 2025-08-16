using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Avalonia.Collections;

public interface IAvaloniaReadOnlyList<out T> : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
}
