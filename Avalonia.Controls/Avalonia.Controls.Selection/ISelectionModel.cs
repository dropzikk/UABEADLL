using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Avalonia.Controls.Selection;

public interface ISelectionModel : INotifyPropertyChanged
{
	IEnumerable? Source { get; set; }

	bool SingleSelect { get; set; }

	int SelectedIndex { get; set; }

	IReadOnlyList<int> SelectedIndexes { get; }

	object? SelectedItem { get; set; }

	IReadOnlyList<object?> SelectedItems { get; }

	int AnchorIndex { get; set; }

	int Count { get; }

	event EventHandler<SelectionModelIndexesChangedEventArgs>? IndexesChanged;

	event EventHandler<SelectionModelSelectionChangedEventArgs>? SelectionChanged;

	event EventHandler? LostSelection;

	event EventHandler? SourceReset;

	void BeginBatchUpdate();

	void EndBatchUpdate();

	bool IsSelected(int index);

	void Select(int index);

	void Deselect(int index);

	void SelectRange(int start, int end);

	void DeselectRange(int start, int end);

	void SelectAll();

	void Clear();
}
