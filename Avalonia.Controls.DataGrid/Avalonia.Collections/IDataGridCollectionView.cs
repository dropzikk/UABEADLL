using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace Avalonia.Collections;

internal interface IDataGridCollectionView : IEnumerable, INotifyCollectionChanged
{
	CultureInfo Culture { get; set; }

	IEnumerable SourceCollection { get; }

	Func<object, bool> Filter { get; set; }

	bool CanFilter { get; }

	DataGridSortDescriptionCollection SortDescriptions { get; }

	bool CanSort { get; }

	bool CanGroup { get; }

	bool IsGrouping { get; }

	int GroupingDepth { get; }

	IAvaloniaReadOnlyList<object> Groups { get; }

	bool IsEmpty { get; }

	object CurrentItem { get; }

	int CurrentPosition { get; }

	bool IsCurrentAfterLast { get; }

	bool IsCurrentBeforeFirst { get; }

	event EventHandler<DataGridCurrentChangingEventArgs> CurrentChanging;

	event EventHandler CurrentChanged;

	bool Contains(object item);

	string GetGroupingPropertyNameAtDepth(int level);

	void Refresh();

	IDisposable DeferRefresh();

	bool MoveCurrentToFirst();

	bool MoveCurrentToLast();

	bool MoveCurrentToNext();

	bool MoveCurrentToPrevious();

	bool MoveCurrentTo(object item);

	bool MoveCurrentToPosition(int position);
}
