using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Search;

public class SearchPanel : TemplatedControl, IRoutedCommandBindable
{
	private TextArea _textArea;

	private SearchInputHandler _handler;

	private TextDocument _currentDocument;

	private SearchResultBackgroundRenderer _renderer;

	private TextBox _searchTextBox;

	private Border _border;

	private int _currentSearchResultIndex = -1;

	public static readonly StyledProperty<bool> UseRegexProperty;

	public static readonly StyledProperty<bool> MatchCaseProperty;

	public static readonly StyledProperty<bool> WholeWordsProperty;

	public static readonly StyledProperty<string> SearchPatternProperty;

	public static readonly StyledProperty<bool> IsReplaceModeProperty;

	public static readonly StyledProperty<string> ReplacePatternProperty;

	private ISearchStrategy _strategy;

	private Panel _messageView;

	private TextBlock _messageViewContent;

	private TextEditor _textEditor { get; set; }

	public bool UseRegex
	{
		get
		{
			return GetValue(UseRegexProperty);
		}
		set
		{
			SetValue(UseRegexProperty, value);
		}
	}

	public bool MatchCase
	{
		get
		{
			return GetValue(MatchCaseProperty);
		}
		set
		{
			SetValue(MatchCaseProperty, value);
		}
	}

	public bool WholeWords
	{
		get
		{
			return GetValue(WholeWordsProperty);
		}
		set
		{
			SetValue(WholeWordsProperty, value);
		}
	}

	public string SearchPattern
	{
		get
		{
			return GetValue(SearchPatternProperty);
		}
		set
		{
			SetValue(SearchPatternProperty, value);
		}
	}

	public bool IsReplaceMode
	{
		get
		{
			return GetValue(IsReplaceModeProperty);
		}
		set
		{
			StyledProperty<bool> isReplaceModeProperty = IsReplaceModeProperty;
			TextEditor textEditor = _textEditor;
			SetValue(isReplaceModeProperty, (textEditor == null || !textEditor.IsReadOnly) && value);
		}
	}

	public string ReplacePattern
	{
		get
		{
			return GetValue(ReplacePatternProperty);
		}
		set
		{
			SetValue(ReplacePatternProperty, value);
		}
	}

	public TextEditor TextEditor => _textEditor;

	public bool IsClosed { get; private set; }

	public bool IsOpened => !IsClosed;

	public IList<RoutedCommandBinding> CommandBindings { get; } = new List<RoutedCommandBinding>();

	public event EventHandler<SearchOptionsChangedEventArgs> SearchOptionsChanged;

	public void SetSearchResultsBrush(IBrush brush)
	{
		if (_renderer != null)
		{
			_renderer.MarkerBrush = brush;
			_textEditor.TextArea.TextView.InvalidateVisual();
		}
	}

	private static void SearchPatternChangedCallback(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is SearchPanel searchPanel)
		{
			searchPanel.ValidateSearchText();
			searchPanel.UpdateSearch();
		}
	}

	private void UpdateSearch()
	{
		try
		{
			if (_renderer.CurrentResults.Any())
			{
				_messageView.IsVisible = false;
			}
			_strategy = SearchStrategyFactory.Create(SearchPattern ?? "", !MatchCase, WholeWords, UseRegex ? SearchMode.RegEx : SearchMode.Normal);
			OnSearchOptionsChanged(new SearchOptionsChangedEventArgs(SearchPattern, MatchCase, UseRegex, WholeWords));
			DoSearch(changeSelection: true);
		}
		catch (SearchPatternException)
		{
			CleanSearchResults();
			UpdateSearchLabel();
		}
	}

	static SearchPanel()
	{
		UseRegexProperty = AvaloniaProperty.Register<SearchPanel, bool>("UseRegex", defaultValue: false);
		MatchCaseProperty = AvaloniaProperty.Register<SearchPanel, bool>("MatchCase", defaultValue: false);
		WholeWordsProperty = AvaloniaProperty.Register<SearchPanel, bool>("WholeWords", defaultValue: false);
		SearchPatternProperty = AvaloniaProperty.Register<SearchPanel, string>("SearchPattern", "");
		IsReplaceModeProperty = AvaloniaProperty.Register<SearchPanel, bool>("IsReplaceMode", defaultValue: false);
		ReplacePatternProperty = AvaloniaProperty.Register<SearchPanel, string>("ReplacePattern");
		UseRegexProperty.Changed.Subscribe(SearchPatternChangedCallback);
		MatchCaseProperty.Changed.Subscribe(SearchPatternChangedCallback);
		WholeWordsProperty.Changed.Subscribe(SearchPatternChangedCallback);
		SearchPatternProperty.Changed.Subscribe(SearchPatternChangedCallback);
	}

	public static SearchPanel Install(TextEditor editor)
	{
		if (editor == null)
		{
			throw new ArgumentNullException("editor");
		}
		if (editor.TextArea == null)
		{
			throw new ArgumentNullException("TextArea");
		}
		TextArea textArea = editor.TextArea;
		SearchPanel searchPanel = new SearchPanel();
		searchPanel.AttachInternal(editor);
		searchPanel._handler = new SearchInputHandler(textArea, searchPanel);
		textArea.DefaultInputHandler.NestedInputHandlers.Add(searchPanel._handler);
		((ISetLogicalParent)searchPanel).SetParent((ILogical?)textArea);
		return searchPanel;
	}

	public void RegisterCommands(ICollection<RoutedCommandBinding> commandBindings)
	{
		_handler.RegisterGlobalCommands(commandBindings);
	}

	public void Uninstall()
	{
		Close();
		_textArea.DocumentChanged -= TextArea_DocumentChanged;
		if (_currentDocument != null)
		{
			_currentDocument.TextChanged -= TextArea_Document_TextChanged;
		}
		_textArea.DefaultInputHandler.NestedInputHandlers.Remove(_handler);
	}

	private void AttachInternal(TextEditor textEditor)
	{
		_textEditor = textEditor;
		_textArea = textEditor.TextArea;
		_renderer = new SearchResultBackgroundRenderer(textEditor.SearchResultsBrush);
		_currentDocument = _textArea.Document;
		if (_currentDocument != null)
		{
			_currentDocument.TextChanged += TextArea_Document_TextChanged;
		}
		_textArea.DocumentChanged += TextArea_DocumentChanged;
		base.KeyDown += SearchLayerKeyDown;
		CommandBindings.Add(new RoutedCommandBinding(SearchCommands.FindNext, delegate
		{
			FindNext();
		}));
		CommandBindings.Add(new RoutedCommandBinding(SearchCommands.FindPrevious, delegate
		{
			FindPrevious();
		}));
		CommandBindings.Add(new RoutedCommandBinding(SearchCommands.CloseSearchPanel, delegate
		{
			Close();
		}));
		CommandBindings.Add(new RoutedCommandBinding(ApplicationCommands.Find, delegate
		{
			IsReplaceMode = false;
			Reactivate();
		}));
		CommandBindings.Add(new RoutedCommandBinding(ApplicationCommands.Replace, delegate
		{
			IsReplaceMode = true;
		}));
		CommandBindings.Add(new RoutedCommandBinding(SearchCommands.ReplaceNext, delegate
		{
			ReplaceNext();
		}, delegate(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsReplaceMode;
		}));
		CommandBindings.Add(new RoutedCommandBinding(SearchCommands.ReplaceAll, delegate
		{
			ReplaceAll();
		}, delegate(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsReplaceMode;
		}));
		IsClosed = true;
	}

	private void TextArea_DocumentChanged(object sender, EventArgs e)
	{
		if (_currentDocument != null)
		{
			_currentDocument.TextChanged -= TextArea_Document_TextChanged;
		}
		_currentDocument = _textArea.Document;
		if (_currentDocument != null)
		{
			_currentDocument.TextChanged += TextArea_Document_TextChanged;
			DoSearch(changeSelection: false);
		}
	}

	private void TextArea_Document_TextChanged(object sender, EventArgs e)
	{
		DoSearch(changeSelection: false);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_border = e.NameScope.Find<Border>("PART_Border");
		_searchTextBox = e.NameScope.Find<TextBox>("PART_searchTextBox");
		_messageView = e.NameScope.Find<Panel>("PART_MessageView");
		_messageViewContent = e.NameScope.Find<TextBlock>("PART_MessageContent");
	}

	private void ValidateSearchText()
	{
		if (_searchTextBox != null)
		{
			UpdateSearch();
		}
	}

	public void Reactivate()
	{
		if (_searchTextBox != null)
		{
			_searchTextBox.Focus();
			_searchTextBox.SelectionStart = 0;
			_searchTextBox.SelectionEnd = _searchTextBox.Text?.Length ?? 0;
		}
	}

	public void FindNext()
	{
		SearchResult searchResult = _renderer.CurrentResults.FindFirstSegmentWithStartAfter(_textArea.Caret.Offset + 1) ?? _renderer.CurrentResults.FirstSegment;
		if (searchResult != null)
		{
			_currentSearchResultIndex = GetSearchResultIndex(_renderer.CurrentResults, searchResult);
			SelectResult(searchResult);
			UpdateSearchLabel();
		}
	}

	public void FindPrevious()
	{
		SearchResult searchResult = _renderer.CurrentResults.FindFirstSegmentWithStartAfter(_textArea.Caret.Offset);
		if (searchResult != null)
		{
			searchResult = _renderer.CurrentResults.GetPreviousSegment(searchResult);
		}
		if (searchResult == null)
		{
			searchResult = _renderer.CurrentResults.LastSegment;
		}
		if (searchResult != null)
		{
			_currentSearchResultIndex = GetSearchResultIndex(_renderer.CurrentResults, searchResult);
			SelectResult(searchResult);
			UpdateSearchLabel();
		}
	}

	public void ReplaceNext()
	{
		if (IsReplaceMode)
		{
			FindNext();
			if (!_textArea.Selection.IsEmpty)
			{
				_textArea.Selection.ReplaceSelectionWithText(ReplacePattern ?? string.Empty);
			}
			UpdateSearch();
		}
	}

	public void ReplaceAll()
	{
		if (!IsReplaceMode)
		{
			return;
		}
		string text = ReplacePattern ?? string.Empty;
		TextDocument document = _textArea.Document;
		using (document.RunUpdate())
		{
			SearchResult[] array = _renderer.CurrentResults.OrderByDescending((SearchResult x) => x.EndOffset).ToArray();
			foreach (SearchResult searchResult in array)
			{
				document.Replace(searchResult.StartOffset, searchResult.Length, new StringTextSource(text));
			}
		}
	}

	private void DoSearch(bool changeSelection)
	{
		if (IsClosed)
		{
			return;
		}
		CleanSearchResults();
		if (!string.IsNullOrEmpty(SearchPattern))
		{
			int offset = _textArea.Caret.Offset;
			if (changeSelection)
			{
				_textArea.ClearSelection();
			}
			foreach (SearchResult item in _strategy.FindAll(_textArea.Document, 0, _textArea.Document.TextLength).Cast<SearchResult>())
			{
				_renderer.CurrentResults.Add(item);
				if (changeSelection && item.StartOffset >= offset)
				{
					SelectResult(item);
					_currentSearchResultIndex = _renderer.CurrentResults.Count - 1;
					changeSelection = false;
				}
			}
		}
		UpdateSearchLabel();
		_textArea.TextView.InvalidateLayer(KnownLayer.Selection);
	}

	private void CleanSearchResults()
	{
		_renderer.CurrentResults.Clear();
		_currentSearchResultIndex = -1;
	}

	private void UpdateSearchLabel()
	{
		if (_messageView == null || _messageViewContent == null)
		{
			return;
		}
		_messageView.IsVisible = true;
		if (!_renderer.CurrentResults.Any())
		{
			_messageViewContent.Text = SR.SearchNoMatchesFoundText;
		}
		else if (_currentSearchResultIndex == -1)
		{
			if (_renderer.CurrentResults.Count == 1)
			{
				_messageViewContent.Text = SR.Search1Match;
			}
			else
			{
				_messageViewContent.Text = string.Format(SR.SearchXMatches, _renderer.CurrentResults.Count);
			}
		}
		else
		{
			_messageViewContent.Text = string.Format(SR.SearchXOfY, _currentSearchResultIndex + 1, _renderer.CurrentResults.Count);
		}
	}

	private void SelectResult(TextSegment result)
	{
		_textArea.Caret.Offset = result.StartOffset;
		_textArea.Selection = Selection.Create(_textArea, result.StartOffset, result.EndOffset);
		double border = ((_border == null) ? 0.0 : (_border.Bounds.Height + _textArea.TextView.DefaultLineHeight));
		_textArea.Caret.BringCaretToView(border);
		_textArea.Caret.Show();
	}

	private void SearchLayerKeyDown(object sender, KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Return:
			e.Handled = true;
			if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
			{
				FindPrevious();
			}
			else
			{
				FindNext();
			}
			break;
		case Key.Escape:
			e.Handled = true;
			Close();
			break;
		}
	}

	public void Close()
	{
		_textArea.RemoveChild(this);
		_messageView.IsVisible = false;
		_textArea.TextView.BackgroundRenderers.Remove(_renderer);
		IsClosed = true;
		_renderer.CurrentResults.Clear();
		_currentSearchResultIndex = -1;
		_textArea.Focus();
	}

	public void Open()
	{
		if (IsClosed)
		{
			_textArea.AddChild(this);
			_textArea.TextView.BackgroundRenderers.Add(_renderer);
			IsClosed = false;
			DoSearch(changeSelection: false);
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		e.Handled = true;
		base.OnPointerPressed(e);
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		base.Cursor = Avalonia.Input.Cursor.Default;
		base.OnPointerMoved(e);
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		e.Handled = true;
		base.OnGotFocus(e);
	}

	private static int GetSearchResultIndex(TextSegmentCollection<SearchResult> searchResults, SearchResult match)
	{
		int num = 0;
		foreach (SearchResult searchResult in searchResults)
		{
			if (searchResult.Equals(match))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	protected virtual void OnSearchOptionsChanged(SearchOptionsChangedEventArgs e)
	{
		this.SearchOptionsChanged?.Invoke(this, e);
	}
}
