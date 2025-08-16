using System;
using System.Collections;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls.Utils;

public interface ISelectionAdapter
{
	object? SelectedItem { get; set; }

	IEnumerable? ItemsSource { get; set; }

	event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

	event EventHandler<RoutedEventArgs>? Commit;

	event EventHandler<RoutedEventArgs>? Cancel;

	void HandleKeyDown(KeyEventArgs e);
}
