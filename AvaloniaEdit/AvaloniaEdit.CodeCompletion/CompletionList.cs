using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.CodeCompletion;

public class CompletionList : TemplatedControl
{
	public static readonly StyledProperty<ControlTemplate> EmptyTemplateProperty = AvaloniaProperty.Register<CompletionList, ControlTemplate>("EmptyTemplate");

	private CompletionListBox _listBox;

	private readonly ObservableCollection<ICompletionData> _completionData = new ObservableCollection<ICompletionData>();

	private string _currentText;

	private ObservableCollection<ICompletionData> _currentList;

	public bool IsFiltering { get; set; } = true;

	public ControlTemplate EmptyTemplate
	{
		get
		{
			return GetValue(EmptyTemplateProperty);
		}
		set
		{
			SetValue(EmptyTemplateProperty, value);
		}
	}

	public CompletionListBox ListBox
	{
		get
		{
			if (_listBox == null)
			{
				ApplyTemplate();
			}
			return _listBox;
		}
	}

	public Key[] CompletionAcceptKeys { get; set; }

	public ScrollViewer ScrollViewer => _listBox?.ScrollViewer;

	public IList<ICompletionData> CompletionData => _completionData;

	public ICompletionData SelectedItem
	{
		get
		{
			return _listBox?.SelectedItem as ICompletionData;
		}
		set
		{
			if (_listBox == null && value != null)
			{
				ApplyTemplate();
			}
			if (_listBox != null)
			{
				_listBox.SelectedItem = value;
			}
		}
	}

	public List<ICompletionData> CurrentList => ListBox.Items.Cast<ICompletionData>().ToList();

	public event EventHandler InsertionRequested;

	public event EventHandler<SelectionChangedEventArgs> SelectionChanged
	{
		add
		{
			AddHandler(SelectingItemsControl.SelectionChangedEvent, value);
		}
		remove
		{
			RemoveHandler(SelectingItemsControl.SelectionChangedEvent, value);
		}
	}

	public CompletionList()
	{
		base.DoubleTapped += OnDoubleTapped;
		CompletionAcceptKeys = new Key[2]
		{
			Key.Return,
			Key.Tab
		};
	}

	public void RequestInsertion(EventArgs e)
	{
		this.InsertionRequested?.Invoke(this, e);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_listBox = e.NameScope.Find("PART_ListBox") as CompletionListBox;
		if (_listBox != null)
		{
			_listBox.ItemsSource = _completionData;
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled)
		{
			HandleKey(e);
		}
	}

	public void HandleKey(KeyEventArgs e)
	{
		if (_listBox == null)
		{
			return;
		}
		switch (e.Key)
		{
		case Key.Down:
			e.Handled = true;
			_listBox.SelectIndex(_listBox.SelectedIndex + 1);
			return;
		case Key.Up:
			e.Handled = true;
			_listBox.SelectIndex(_listBox.SelectedIndex - 1);
			return;
		case Key.PageDown:
			e.Handled = true;
			_listBox.SelectIndex(_listBox.SelectedIndex + _listBox.VisibleItemCount);
			return;
		case Key.PageUp:
			e.Handled = true;
			_listBox.SelectIndex(_listBox.SelectedIndex - _listBox.VisibleItemCount);
			return;
		case Key.Home:
			e.Handled = true;
			_listBox.SelectIndex(0);
			return;
		case Key.End:
			e.Handled = true;
			_listBox.SelectIndex(_listBox.ItemCount - 1);
			return;
		}
		if (CompletionAcceptKeys.Contains(e.Key) && CurrentList.Count > 0)
		{
			e.Handled = true;
			RequestInsertion(e);
		}
	}

	protected void OnDoubleTapped(object sender, RoutedEventArgs e)
	{
		if (((AvaloniaObject)e.Source).VisualAncestorsAndSelf().TakeWhile((AvaloniaObject obj) => obj != this).Any((AvaloniaObject obj) => obj is ListBoxItem))
		{
			e.Handled = true;
			RequestInsertion(e);
		}
	}

	public void ScrollIntoView(ICompletionData item)
	{
		if (_listBox == null)
		{
			ApplyTemplate();
		}
		_listBox?.ScrollIntoView(item);
	}

	public void SelectItem(string text)
	{
		if (!(text == _currentText))
		{
			if (_listBox == null)
			{
				ApplyTemplate();
			}
			if (IsFiltering)
			{
				SelectItemFiltering(text);
			}
			else
			{
				SelectItemWithStart(text);
			}
			_currentText = text;
		}
	}

	private void SelectItemFiltering(string query)
	{
		var enumerable = from item in (_currentList != null && !string.IsNullOrEmpty(_currentText) && !string.IsNullOrEmpty(query) && query.StartsWith(_currentText, StringComparison.Ordinal)) ? _currentList : _completionData
			let quality = GetMatchQuality(item.Text, query)
			where quality > 0
			select new
			{
				Item = item,
				Quality = quality
			};
		ICompletionData completionData = ((_listBox.SelectedIndex != -1) ? ((ICompletionData)_listBox.SelectedItem) : null);
		ObservableCollection<ICompletionData> observableCollection = new ObservableCollection<ICompletionData>();
		int bestIndex = -1;
		int num = -1;
		double num2 = 0.0;
		int num3 = 0;
		foreach (var item in enumerable)
		{
			double num4 = ((item.Item == completionData) ? double.PositiveInfinity : item.Item.Priority);
			int quality = item.Quality;
			if (quality > num || (quality == num && num4 > num2))
			{
				bestIndex = num3;
				num2 = num4;
				num = quality;
			}
			observableCollection.Add(item.Item);
			num3++;
		}
		_currentList = observableCollection;
		_listBox.ItemsSource = observableCollection;
		SelectIndexCentered(bestIndex);
	}

	private void SelectItemWithStart(string query)
	{
		if (string.IsNullOrEmpty(query))
		{
			return;
		}
		int selectedIndex = _listBox.SelectedIndex;
		int num = -1;
		int num2 = -1;
		double num3 = 0.0;
		for (int i = 0; i < _completionData.Count; i++)
		{
			int matchQuality = GetMatchQuality(_completionData[i].Text, query);
			if (matchQuality >= 0)
			{
				double priority = _completionData[i].Priority;
				if (num2 < matchQuality || (num != selectedIndex && ((i != selectedIndex) ? (num2 == matchQuality && num3 < priority) : (num2 == matchQuality))))
				{
					num = i;
					num3 = priority;
					num2 = matchQuality;
				}
			}
		}
		SelectIndexCentered(num);
	}

	private void SelectIndexCentered(int bestIndex)
	{
		if (bestIndex < 0)
		{
			_listBox.ClearSelection();
			return;
		}
		int firstVisibleItem = _listBox.FirstVisibleItem;
		if (bestIndex < firstVisibleItem || firstVisibleItem + _listBox.VisibleItemCount <= bestIndex)
		{
			_listBox.CenterViewOn(bestIndex);
			_listBox.SelectIndex(bestIndex);
		}
		else
		{
			_listBox.SelectIndex(bestIndex);
		}
	}

	private int GetMatchQuality(string itemText, string query)
	{
		if (itemText == null)
		{
			throw new ArgumentNullException("itemText", "ICompletionData.Text returned null");
		}
		if (query == itemText)
		{
			return 8;
		}
		if (string.Equals(itemText, query, StringComparison.CurrentCultureIgnoreCase))
		{
			return 7;
		}
		if (itemText.StartsWith(query, StringComparison.CurrentCulture))
		{
			return 6;
		}
		if (itemText.StartsWith(query, StringComparison.CurrentCultureIgnoreCase))
		{
			return 5;
		}
		bool? flag = null;
		if (query.Length <= 2)
		{
			flag = CamelCaseMatch(itemText, query);
			if (flag == true)
			{
				return 4;
			}
		}
		if (IsFiltering)
		{
			if (itemText.IndexOf(query, StringComparison.CurrentCulture) >= 0)
			{
				return 3;
			}
			if (itemText.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0)
			{
				return 2;
			}
		}
		if (!flag.HasValue)
		{
			flag = CamelCaseMatch(itemText, query);
		}
		if (flag == true)
		{
			return 1;
		}
		return -1;
	}

	private static bool CamelCaseMatch(string text, string query)
	{
		IEnumerable<char> enumerable = text.AsEnumerable().Take(1).Concat(text.AsEnumerable().Skip(1).Where(char.IsUpper));
		int num = 0;
		foreach (char item in enumerable)
		{
			if (num > query.Length - 1)
			{
				return true;
			}
			if (char.ToUpperInvariant(query[num]) != char.ToUpperInvariant(item))
			{
				return false;
			}
			num++;
		}
		if (num >= query.Length)
		{
			return true;
		}
		return false;
	}
}
