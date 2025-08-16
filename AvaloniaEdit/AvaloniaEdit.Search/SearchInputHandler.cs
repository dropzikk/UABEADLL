using System;
using System.Collections.Generic;
using Avalonia.Threading;
using AvaloniaEdit.Editing;

namespace AvaloniaEdit.Search;

internal class SearchInputHandler : TextAreaInputHandler
{
	private readonly SearchPanel _panel;

	public event EventHandler<SearchOptionsChangedEventArgs> SearchOptionsChanged
	{
		add
		{
			_panel.SearchOptionsChanged += value;
		}
		remove
		{
			_panel.SearchOptionsChanged -= value;
		}
	}

	public SearchInputHandler(TextArea textArea, SearchPanel panel)
		: base(textArea)
	{
		RegisterCommands(base.CommandBindings);
		_panel = panel;
	}

	internal void RegisterGlobalCommands(ICollection<RoutedCommandBinding> commandBindings)
	{
		commandBindings.Add(new RoutedCommandBinding(ApplicationCommands.Find, ExecuteFind));
		base.CommandBindings.Add(new RoutedCommandBinding(ApplicationCommands.Replace, ExecuteReplace));
		commandBindings.Add(new RoutedCommandBinding(SearchCommands.FindNext, ExecuteFindNext, CanExecuteWithOpenSearchPanel));
		commandBindings.Add(new RoutedCommandBinding(SearchCommands.FindPrevious, ExecuteFindPrevious, CanExecuteWithOpenSearchPanel));
	}

	private void RegisterCommands(ICollection<RoutedCommandBinding> commandBindings)
	{
		commandBindings.Add(new RoutedCommandBinding(ApplicationCommands.Find, ExecuteFind));
		base.CommandBindings.Add(new RoutedCommandBinding(ApplicationCommands.Replace, ExecuteReplace));
		commandBindings.Add(new RoutedCommandBinding(SearchCommands.FindNext, ExecuteFindNext, CanExecuteWithOpenSearchPanel));
		commandBindings.Add(new RoutedCommandBinding(SearchCommands.FindPrevious, ExecuteFindPrevious, CanExecuteWithOpenSearchPanel));
		base.CommandBindings.Add(new RoutedCommandBinding(SearchCommands.ReplaceNext, ExecuteReplaceNext, CanExecuteWithOpenSearchPanel));
		base.CommandBindings.Add(new RoutedCommandBinding(SearchCommands.ReplaceAll, ExecuteReplaceAll, CanExecuteWithOpenSearchPanel));
		commandBindings.Add(new RoutedCommandBinding(SearchCommands.CloseSearchPanel, ExecuteCloseSearchPanel, CanExecuteWithOpenSearchPanel));
	}

	private void ExecuteFind(object sender, ExecutedRoutedEventArgs e)
	{
		FindOrReplace(isReplaceMode: false);
	}

	private void ExecuteReplace(object sender, ExecutedRoutedEventArgs e)
	{
		FindOrReplace(isReplaceMode: true);
	}

	private void FindOrReplace(bool isReplaceMode)
	{
		_panel.IsReplaceMode = isReplaceMode;
		_panel.Open();
		if (!base.TextArea.Selection.IsEmpty && !base.TextArea.Selection.IsMultiline)
		{
			_panel.SearchPattern = base.TextArea.Selection.GetText();
		}
		Dispatcher.UIThread.Post(_panel.Reactivate, DispatcherPriority.Input);
	}

	private void CanExecuteWithOpenSearchPanel(object sender, CanExecuteRoutedEventArgs e)
	{
		if (_panel.IsClosed)
		{
			e.CanExecute = false;
			return;
		}
		e.CanExecute = true;
		e.Handled = true;
	}

	private void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e)
	{
		if (!_panel.IsClosed)
		{
			_panel.FindNext();
			e.Handled = true;
		}
	}

	private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e)
	{
		if (!_panel.IsClosed)
		{
			_panel.FindPrevious();
			e.Handled = true;
		}
	}

	private void ExecuteReplaceNext(object sender, ExecutedRoutedEventArgs e)
	{
		if (!_panel.IsClosed)
		{
			_panel.ReplaceNext();
			e.Handled = true;
		}
	}

	private void ExecuteReplaceAll(object sender, ExecutedRoutedEventArgs e)
	{
		if (!_panel.IsClosed)
		{
			_panel.ReplaceAll();
			e.Handled = true;
		}
	}

	private void ExecuteCloseSearchPanel(object sender, ExecutedRoutedEventArgs e)
	{
		if (!_panel.IsClosed)
		{
			_panel.Close();
			e.Handled = true;
		}
	}
}
