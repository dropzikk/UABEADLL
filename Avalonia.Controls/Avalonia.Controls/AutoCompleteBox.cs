using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Utils;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

[TemplatePart("PART_Popup", typeof(Popup))]
[TemplatePart("PART_SelectingItemsControl", typeof(SelectingItemsControl))]
[TemplatePart("PART_SelectionAdapter", typeof(ISelectionAdapter))]
[TemplatePart("PART_TextBox", typeof(TextBox))]
[PseudoClasses(new string[] { ":dropdownopen" })]
public class AutoCompleteBox : TemplatedControl
{
	private static class AutoCompleteSearch
	{
		public static AutoCompleteFilterPredicate<string?>? GetFilter(AutoCompleteFilterMode FilterMode)
		{
			return FilterMode switch
			{
				AutoCompleteFilterMode.Contains => Contains, 
				AutoCompleteFilterMode.ContainsCaseSensitive => ContainsCaseSensitive, 
				AutoCompleteFilterMode.ContainsOrdinal => ContainsOrdinal, 
				AutoCompleteFilterMode.ContainsOrdinalCaseSensitive => ContainsOrdinalCaseSensitive, 
				AutoCompleteFilterMode.Equals => Equals, 
				AutoCompleteFilterMode.EqualsCaseSensitive => EqualsCaseSensitive, 
				AutoCompleteFilterMode.EqualsOrdinal => EqualsOrdinal, 
				AutoCompleteFilterMode.EqualsOrdinalCaseSensitive => EqualsOrdinalCaseSensitive, 
				AutoCompleteFilterMode.StartsWith => StartsWith, 
				AutoCompleteFilterMode.StartsWithCaseSensitive => StartsWithCaseSensitive, 
				AutoCompleteFilterMode.StartsWithOrdinal => StartsWithOrdinal, 
				AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive => StartsWithOrdinalCaseSensitive, 
				_ => null, 
			};
		}

		private static bool Contains(string? s, string? value, StringComparison comparison)
		{
			if (s != null && value != null)
			{
				return s.IndexOf(value, comparison) >= 0;
			}
			return false;
		}

		public static bool StartsWith(string? text, string? value)
		{
			if (value != null && text != null)
			{
				return value.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);
			}
			return false;
		}

		public static bool StartsWithCaseSensitive(string? text, string? value)
		{
			if (value != null && text != null)
			{
				return value.StartsWith(text, StringComparison.CurrentCulture);
			}
			return false;
		}

		public static bool StartsWithOrdinal(string? text, string? value)
		{
			if (value != null && text != null)
			{
				return value.StartsWith(text, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public static bool StartsWithOrdinalCaseSensitive(string? text, string? value)
		{
			if (value != null && text != null)
			{
				return value.StartsWith(text, StringComparison.Ordinal);
			}
			return false;
		}

		public static bool Contains(string? text, string? value)
		{
			return Contains(value, text, StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool ContainsCaseSensitive(string? text, string? value)
		{
			return Contains(value, text, StringComparison.CurrentCulture);
		}

		public static bool ContainsOrdinal(string? text, string? value)
		{
			return Contains(value, text, StringComparison.OrdinalIgnoreCase);
		}

		public static bool ContainsOrdinalCaseSensitive(string? text, string? value)
		{
			return Contains(value, text, StringComparison.Ordinal);
		}

		public static bool Equals(string? text, string? value)
		{
			return string.Equals(value, text, StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool EqualsCaseSensitive(string? text, string? value)
		{
			return string.Equals(value, text, StringComparison.CurrentCulture);
		}

		public static bool EqualsOrdinal(string? text, string? value)
		{
			return string.Equals(value, text, StringComparison.OrdinalIgnoreCase);
		}

		public static bool EqualsOrdinalCaseSensitive(string? text, string? value)
		{
			return string.Equals(value, text, StringComparison.Ordinal);
		}
	}

	public class BindingEvaluator<T> : Control
	{
		private IBinding? _binding;

		public static readonly StyledProperty<T> ValueProperty = AvaloniaProperty.Register<BindingEvaluator<T>, T>("Value");

		public T Value
		{
			get
			{
				return GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);
			}
		}

		public IBinding? ValueBinding
		{
			get
			{
				return _binding;
			}
			set
			{
				_binding = value;
				if (value != null)
				{
					this.Bind(ValueProperty, value);
				}
			}
		}

		public BindingEvaluator()
		{
		}

		public BindingEvaluator(IBinding? binding)
			: this()
		{
			ValueBinding = binding;
		}

		public void ClearDataContext()
		{
			base.DataContext = null;
		}

		public T GetDynamicValue(object o, bool clearDataContext)
		{
			base.DataContext = o;
			T value = Value;
			if (clearDataContext)
			{
				base.DataContext = null;
			}
			return value;
		}

		public T GetDynamicValue(object? o)
		{
			base.DataContext = o;
			return Value;
		}
	}

	private const string ElementSelectionAdapter = "PART_SelectionAdapter";

	private const string ElementSelector = "PART_SelectingItemsControl";

	private const string ElementPopup = "PART_Popup";

	private const string ElementTextBox = "PART_TextBox";

	private List<object>? _items;

	private AvaloniaList<object>? _view;

	private int _ignoreTextPropertyChange;

	private bool _ignorePropertyChange;

	private bool _ignoreTextSelectionChange;

	private bool _skipSelectedItemTextUpdate;

	private int _textSelectionStart;

	private bool _userCalledPopulate;

	private bool _popupHasOpened;

	private DispatcherTimer? _delayTimer;

	private bool _allowWrite;

	private TextBox? _textBox;

	private IDisposable? _textBoxSubscriptions;

	private ISelectionAdapter? _adapter;

	private BindingEvaluator<string>? _valueBindingEvaluator;

	private IDisposable? _collectionChangeSubscription;

	private CancellationTokenSource? _populationCancellationTokenSource;

	private bool _itemTemplateIsFromValueMemberBinding = true;

	private bool _settingItemTemplateFromValueMemberBinding;

	private bool _isFocused;

	private string? _searchText = string.Empty;

	private readonly EventHandler _populateDropDownHandler;

	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent;

	public static readonly RoutedEvent<TextChangedEventArgs> TextChangedEvent;

	public static readonly StyledProperty<string?> WatermarkProperty;

	public static readonly StyledProperty<int> MinimumPrefixLengthProperty;

	public static readonly StyledProperty<TimeSpan> MinimumPopulateDelayProperty;

	public static readonly StyledProperty<double> MaxDropDownHeightProperty;

	public static readonly StyledProperty<bool> IsTextCompletionEnabledProperty;

	public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty;

	public static readonly StyledProperty<bool> IsDropDownOpenProperty;

	public static readonly StyledProperty<object?> SelectedItemProperty;

	public static readonly StyledProperty<string?> TextProperty;

	public static readonly DirectProperty<AutoCompleteBox, string?> SearchTextProperty;

	public static readonly StyledProperty<AutoCompleteFilterMode> FilterModeProperty;

	public static readonly StyledProperty<AutoCompleteFilterPredicate<object?>?> ItemFilterProperty;

	public static readonly StyledProperty<AutoCompleteFilterPredicate<string?>?> TextFilterProperty;

	public static readonly StyledProperty<AutoCompleteSelector<object>?> ItemSelectorProperty;

	public static readonly StyledProperty<AutoCompleteSelector<string?>?> TextSelectorProperty;

	public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty;

	public static readonly StyledProperty<Func<string?, CancellationToken, Task<IEnumerable<object>>>?> AsyncPopulatorProperty;

	private Popup? DropDownPopup { get; set; }

	private TextBox? TextBox
	{
		get
		{
			return _textBox;
		}
		set
		{
			_textBoxSubscriptions?.Dispose();
			_textBox = value;
			if (_textBox != null)
			{
				_textBoxSubscriptions = _textBox.GetObservable(Avalonia.Controls.TextBox.TextProperty).Skip(1).Subscribe(delegate
				{
					OnTextBoxTextChanged();
				});
				if (Text != null)
				{
					UpdateTextValue(Text);
				}
			}
		}
	}

	private int TextBoxSelectionStart
	{
		get
		{
			if (TextBox != null)
			{
				return Math.Min(TextBox.SelectionStart, TextBox.SelectionEnd);
			}
			return 0;
		}
	}

	private int TextBoxSelectionLength
	{
		get
		{
			if (TextBox != null)
			{
				return Math.Abs(TextBox.SelectionEnd - TextBox.SelectionStart);
			}
			return 0;
		}
	}

	protected ISelectionAdapter? SelectionAdapter
	{
		get
		{
			return _adapter;
		}
		set
		{
			if (_adapter != null)
			{
				_adapter.SelectionChanged -= OnAdapterSelectionChanged;
				_adapter.Commit -= OnAdapterSelectionComplete;
				_adapter.Cancel -= OnAdapterSelectionCanceled;
				_adapter.Cancel -= OnAdapterSelectionComplete;
				_adapter.ItemsSource = null;
			}
			_adapter = value;
			if (_adapter != null)
			{
				_adapter.SelectionChanged += OnAdapterSelectionChanged;
				_adapter.Commit += OnAdapterSelectionComplete;
				_adapter.Cancel += OnAdapterSelectionCanceled;
				_adapter.Cancel += OnAdapterSelectionComplete;
				_adapter.ItemsSource = _view;
			}
		}
	}

	public int MinimumPrefixLength
	{
		get
		{
			return GetValue(MinimumPrefixLengthProperty);
		}
		set
		{
			SetValue(MinimumPrefixLengthProperty, value);
		}
	}

	public bool IsTextCompletionEnabled
	{
		get
		{
			return GetValue(IsTextCompletionEnabledProperty);
		}
		set
		{
			SetValue(IsTextCompletionEnabledProperty, value);
		}
	}

	public IDataTemplate ItemTemplate
	{
		get
		{
			return GetValue(ItemTemplateProperty);
		}
		set
		{
			SetValue(ItemTemplateProperty, value);
		}
	}

	public TimeSpan MinimumPopulateDelay
	{
		get
		{
			return GetValue(MinimumPopulateDelayProperty);
		}
		set
		{
			SetValue(MinimumPopulateDelayProperty, value);
		}
	}

	public double MaxDropDownHeight
	{
		get
		{
			return GetValue(MaxDropDownHeightProperty);
		}
		set
		{
			SetValue(MaxDropDownHeightProperty, value);
		}
	}

	public bool IsDropDownOpen
	{
		get
		{
			return GetValue(IsDropDownOpenProperty);
		}
		set
		{
			SetValue(IsDropDownOpenProperty, value);
		}
	}

	[AssignBinding]
	public IBinding? ValueMemberBinding
	{
		get
		{
			return _valueBindingEvaluator?.ValueBinding;
		}
		set
		{
			if (ValueMemberBinding != value)
			{
				_valueBindingEvaluator = new BindingEvaluator<string>(value);
				OnValueMemberBindingChanged(value);
			}
		}
	}

	public object? SelectedItem
	{
		get
		{
			return GetValue(SelectedItemProperty);
		}
		set
		{
			SetValue(SelectedItemProperty, value);
		}
	}

	public string? Text
	{
		get
		{
			return GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	public string? SearchText
	{
		get
		{
			return _searchText;
		}
		private set
		{
			try
			{
				_allowWrite = true;
				SetAndRaise(SearchTextProperty, ref _searchText, value);
			}
			finally
			{
				_allowWrite = false;
			}
		}
	}

	public AutoCompleteFilterMode FilterMode
	{
		get
		{
			return GetValue(FilterModeProperty);
		}
		set
		{
			SetValue(FilterModeProperty, value);
		}
	}

	public string? Watermark
	{
		get
		{
			return GetValue(WatermarkProperty);
		}
		set
		{
			SetValue(WatermarkProperty, value);
		}
	}

	public AutoCompleteFilterPredicate<object?>? ItemFilter
	{
		get
		{
			return GetValue(ItemFilterProperty);
		}
		set
		{
			SetValue(ItemFilterProperty, value);
		}
	}

	public AutoCompleteFilterPredicate<string?>? TextFilter
	{
		get
		{
			return GetValue(TextFilterProperty);
		}
		set
		{
			SetValue(TextFilterProperty, value);
		}
	}

	public AutoCompleteSelector<object>? ItemSelector
	{
		get
		{
			return GetValue(ItemSelectorProperty);
		}
		set
		{
			SetValue(ItemSelectorProperty, value);
		}
	}

	public AutoCompleteSelector<string?>? TextSelector
	{
		get
		{
			return GetValue(TextSelectorProperty);
		}
		set
		{
			SetValue(TextSelectorProperty, value);
		}
	}

	public Func<string?, CancellationToken, Task<IEnumerable<object>>>? AsyncPopulator
	{
		get
		{
			return GetValue(AsyncPopulatorProperty);
		}
		set
		{
			SetValue(AsyncPopulatorProperty, value);
		}
	}

	public IEnumerable? ItemsSource
	{
		get
		{
			return GetValue(ItemsSourceProperty);
		}
		set
		{
			SetValue(ItemsSourceProperty, value);
		}
	}

	public event EventHandler<TextChangedEventArgs>? TextChanged
	{
		add
		{
			AddHandler(TextChangedEvent, value);
		}
		remove
		{
			RemoveHandler(TextChangedEvent, value);
		}
	}

	public event EventHandler<PopulatingEventArgs>? Populating;

	public event EventHandler<PopulatedEventArgs>? Populated;

	public event EventHandler<CancelEventArgs>? DropDownOpening;

	public event EventHandler? DropDownOpened;

	public event EventHandler<CancelEventArgs>? DropDownClosing;

	public event EventHandler? DropDownClosed;

	public event EventHandler<SelectionChangedEventArgs> SelectionChanged
	{
		add
		{
			AddHandler(SelectionChangedEvent, value);
		}
		remove
		{
			RemoveHandler(SelectionChangedEvent, value);
		}
	}

	private static bool IsValidMinimumPrefixLength(int value)
	{
		return value >= -1;
	}

	private static bool IsValidMinimumPopulateDelay(TimeSpan value)
	{
		return value.TotalMilliseconds >= 0.0;
	}

	private static bool IsValidMaxDropDownHeight(double value)
	{
		return value >= 0.0;
	}

	private static bool IsValidFilterMode(AutoCompleteFilterMode mode)
	{
		if ((uint)mode <= 13u)
		{
			return true;
		}
		return false;
	}

	private void OnControlIsEnabledChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!(bool)e.NewValue)
		{
			SetCurrentValue(IsDropDownOpenProperty, value: false);
		}
	}

	private void OnMinimumPopulateDelayChanged(AvaloniaPropertyChangedEventArgs e)
	{
		TimeSpan timeSpan = (TimeSpan)e.NewValue;
		if (_delayTimer != null)
		{
			_delayTimer.Stop();
			if (timeSpan == TimeSpan.Zero)
			{
				_delayTimer.Tick -= _populateDropDownHandler;
				_delayTimer = null;
			}
		}
		if (timeSpan > TimeSpan.Zero)
		{
			if (_delayTimer == null)
			{
				_delayTimer = new DispatcherTimer();
				_delayTimer.Tick += _populateDropDownHandler;
			}
			_delayTimer.Interval = timeSpan;
		}
	}

	private void OnIsDropDownOpenChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_ignorePropertyChange)
		{
			_ignorePropertyChange = false;
			return;
		}
		bool oldValue = (bool)e.OldValue;
		if ((bool)e.NewValue)
		{
			TextUpdated(Text, userInitiated: true);
		}
		else
		{
			ClosingDropDown(oldValue);
		}
		UpdatePseudoClasses();
	}

	private void OnSelectedItemPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_ignorePropertyChange)
		{
			_ignorePropertyChange = false;
			return;
		}
		if (_skipSelectedItemTextUpdate)
		{
			_skipSelectedItemTextUpdate = false;
		}
		else
		{
			OnSelectedItemChanged(e.NewValue);
		}
		List<object> list = new List<object>();
		if (e.OldValue != null)
		{
			list.Add(e.OldValue);
		}
		List<object> list2 = new List<object>();
		if (e.NewValue != null)
		{
			list2.Add(e.NewValue);
		}
		OnSelectionChanged(new SelectionChangedEventArgs(SelectionChangedEvent, list, list2));
	}

	private void OnTextPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		TextUpdated((string)e.NewValue, userInitiated: false);
	}

	private void OnSearchTextPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (_ignorePropertyChange)
		{
			_ignorePropertyChange = false;
		}
		else if (!_allowWrite)
		{
			_ignorePropertyChange = true;
			SetCurrentValue(e.Property, e.OldValue);
			throw new InvalidOperationException("Cannot set read-only property SearchText.");
		}
	}

	private void OnFilterModePropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		AutoCompleteFilterMode filterMode = (AutoCompleteFilterMode)e.NewValue;
		SetCurrentValue(TextFilterProperty, AutoCompleteSearch.GetFilter(filterMode));
	}

	private void OnItemFilterPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!(e.NewValue is AutoCompleteFilterPredicate<object>))
		{
			SetCurrentValue(FilterModeProperty, AutoCompleteFilterMode.None);
			return;
		}
		SetCurrentValue(FilterModeProperty, AutoCompleteFilterMode.Custom);
		SetCurrentValue(TextFilterProperty, null);
	}

	private void OnItemsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		OnItemsSourceChanged((IEnumerable)e.NewValue);
	}

	private void OnItemTemplatePropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!_settingItemTemplateFromValueMemberBinding)
		{
			_itemTemplateIsFromValueMemberBinding = false;
		}
	}

	private void OnValueMemberBindingChanged(IBinding? value)
	{
		if (!_itemTemplateIsFromValueMemberBinding)
		{
			return;
		}
		FuncDataTemplate value2 = new FuncDataTemplate(typeof(object), delegate
		{
			ContentControl contentControl = new ContentControl();
			if (value != null)
			{
				contentControl.Bind(ContentControl.ContentProperty, value);
			}
			return contentControl;
		});
		_settingItemTemplateFromValueMemberBinding = true;
		SetCurrentValue(ItemTemplateProperty, value2);
		_settingItemTemplateFromValueMemberBinding = false;
	}

	static AutoCompleteBox()
	{
		SelectionChangedEvent = RoutedEvent.Register<SelectionChangedEventArgs>("SelectionChanged", RoutingStrategies.Bubble, typeof(AutoCompleteBox));
		TextChangedEvent = RoutedEvent.Register<AutoCompleteBox, TextChangedEventArgs>("TextChanged", RoutingStrategies.Bubble);
		WatermarkProperty = Avalonia.Controls.TextBox.WatermarkProperty.AddOwner<AutoCompleteBox>();
		MinimumPrefixLengthProperty = AvaloniaProperty.Register<AutoCompleteBox, int>("MinimumPrefixLength", 1, inherits: false, BindingMode.OneWay, IsValidMinimumPrefixLength);
		MinimumPopulateDelayProperty = AvaloniaProperty.Register<AutoCompleteBox, TimeSpan>("MinimumPopulateDelay", TimeSpan.Zero, inherits: false, BindingMode.OneWay, IsValidMinimumPopulateDelay);
		MaxDropDownHeightProperty = AvaloniaProperty.Register<AutoCompleteBox, double>("MaxDropDownHeight", double.PositiveInfinity, inherits: false, BindingMode.OneWay, IsValidMaxDropDownHeight);
		IsTextCompletionEnabledProperty = AvaloniaProperty.Register<AutoCompleteBox, bool>("IsTextCompletionEnabled", defaultValue: false);
		ItemTemplateProperty = AvaloniaProperty.Register<AutoCompleteBox, IDataTemplate>("ItemTemplate");
		IsDropDownOpenProperty = AvaloniaProperty.Register<AutoCompleteBox, bool>("IsDropDownOpen", defaultValue: false);
		SelectedItemProperty = AvaloniaProperty.Register<AutoCompleteBox, object>("SelectedItem", null, inherits: false, BindingMode.TwoWay, null, null, enableDataValidation: true);
		TextProperty = TextBlock.TextProperty.AddOwner<AutoCompleteBox>(new StyledPropertyMetadata<string>(string.Empty, BindingMode.TwoWay, null, enableDataValidation: true));
		SearchTextProperty = AvaloniaProperty.RegisterDirect<AutoCompleteBox, string>("SearchText", (AutoCompleteBox o) => o.SearchText, null, string.Empty);
		FilterModeProperty = AvaloniaProperty.Register<AutoCompleteBox, AutoCompleteFilterMode>("FilterMode", AutoCompleteFilterMode.StartsWith, inherits: false, BindingMode.OneWay, IsValidFilterMode);
		ItemFilterProperty = AvaloniaProperty.Register<AutoCompleteBox, AutoCompleteFilterPredicate<object>>("ItemFilter");
		TextFilterProperty = AvaloniaProperty.Register<AutoCompleteBox, AutoCompleteFilterPredicate<string>>("TextFilter", AutoCompleteSearch.GetFilter(AutoCompleteFilterMode.StartsWith));
		ItemSelectorProperty = AvaloniaProperty.Register<AutoCompleteBox, AutoCompleteSelector<object>>("ItemSelector");
		TextSelectorProperty = AvaloniaProperty.Register<AutoCompleteBox, AutoCompleteSelector<string>>("TextSelector");
		ItemsSourceProperty = AvaloniaProperty.Register<AutoCompleteBox, IEnumerable>("ItemsSource");
		AsyncPopulatorProperty = AvaloniaProperty.Register<AutoCompleteBox, Func<string, CancellationToken, Task<IEnumerable<object>>>>("AsyncPopulator");
		InputElement.FocusableProperty.OverrideDefaultValue<AutoCompleteBox>(defaultValue: true);
		MinimumPopulateDelayProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnMinimumPopulateDelayChanged(e);
		});
		IsDropDownOpenProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnIsDropDownOpenChanged(e);
		});
		SelectedItemProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnSelectedItemPropertyChanged(e);
		});
		TextProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnTextPropertyChanged(e);
		});
		SearchTextProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnSearchTextPropertyChanged(e);
		});
		FilterModeProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnFilterModePropertyChanged(e);
		});
		ItemFilterProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnItemFilterPropertyChanged(e);
		});
		ItemsSourceProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnItemsSourcePropertyChanged(e);
		});
		ItemTemplateProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnItemTemplatePropertyChanged(e);
		});
		InputElement.IsEnabledProperty.Changed.AddClassHandler(delegate(AutoCompleteBox x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnControlIsEnabledChanged(e);
		});
	}

	public AutoCompleteBox()
	{
		_populateDropDownHandler = PopulateDropDown;
		ClearView();
	}

	protected virtual ISelectionAdapter? GetSelectionAdapterPart(INameScope nameScope)
	{
		ISelectionAdapter selectionAdapter = null;
		SelectingItemsControl selectingItemsControl = nameScope.Find<SelectingItemsControl>("PART_SelectingItemsControl");
		if (selectingItemsControl != null)
		{
			selectionAdapter = selectingItemsControl as ISelectionAdapter;
			if (selectionAdapter == null)
			{
				selectionAdapter = new SelectingItemsControlSelectionAdapter(selectingItemsControl);
			}
		}
		if (selectionAdapter == null)
		{
			selectionAdapter = nameScope.Find<ISelectionAdapter>("PART_SelectionAdapter");
		}
		return selectionAdapter;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		if (DropDownPopup != null)
		{
			DropDownPopup.Closed -= DropDownPopup_Closed;
			DropDownPopup = null;
		}
		Popup popup = e.NameScope.Find<Popup>("PART_Popup");
		if (popup != null)
		{
			DropDownPopup = popup;
			DropDownPopup.Closed += DropDownPopup_Closed;
		}
		SelectionAdapter = GetSelectionAdapterPart(e.NameScope);
		TextBox = e.NameScope.Find<TextBox>("PART_TextBox");
		if (IsDropDownOpen && DropDownPopup != null && !DropDownPopup.IsOpen)
		{
			OpeningDropDown(oldValue: false);
		}
		base.OnApplyTemplate(e);
	}

	protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
		if (property == TextProperty || property == SelectedItemProperty)
		{
			DataValidationErrors.SetError(this, error);
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		base.OnKeyDown(e);
		if (e.Handled || !base.IsEnabled)
		{
			return;
		}
		if (IsDropDownOpen)
		{
			if (SelectionAdapter != null)
			{
				SelectionAdapter.HandleKeyDown(e);
				if (e.Handled)
				{
					return;
				}
			}
			if (e.Key == Key.Escape)
			{
				OnAdapterSelectionCanceled(this, new RoutedEventArgs());
				e.Handled = true;
			}
		}
		else if (e.Key == Key.Down)
		{
			SetCurrentValue(IsDropDownOpenProperty, value: true);
			e.Handled = true;
		}
		switch (e.Key)
		{
		case Key.F4:
			SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
			e.Handled = true;
			break;
		case Key.Return:
			if (IsDropDownOpen)
			{
				OnAdapterSelectionComplete(this, new RoutedEventArgs());
				e.Handled = true;
			}
			break;
		}
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		FocusChanged(HasFocus());
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		FocusChanged(HasFocus());
	}

	protected bool HasFocus()
	{
		Visual visual = FocusManager.GetFocusManager(this)?.GetFocusedElement() as Visual;
		while (visual != null)
		{
			if (visual == this)
			{
				return true;
			}
			Visual visualParent = visual.GetVisualParent();
			if (visualParent == null && visual is Control control)
			{
				visualParent = control.VisualParent;
			}
			visual = visualParent;
		}
		return false;
	}

	private void FocusChanged(bool hasFocus)
	{
		bool isFocused = _isFocused;
		_isFocused = hasFocus;
		if (hasFocus)
		{
			if (!isFocused && TextBox != null && TextBoxSelectionLength <= 0)
			{
				TextBox.Focus();
				TextBox.SelectionStart = 0;
				TextBox.SelectionEnd = TextBox.Text?.Length ?? 0;
			}
		}
		else
		{
			SetCurrentValue(IsDropDownOpenProperty, value: false);
			_userCalledPopulate = false;
			ClearTextBoxSelection();
		}
		_isFocused = hasFocus;
	}

	protected virtual void OnPopulating(PopulatingEventArgs e)
	{
		this.Populating?.Invoke(this, e);
	}

	protected virtual void OnPopulated(PopulatedEventArgs e)
	{
		this.Populated?.Invoke(this, e);
	}

	protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
	{
		RaiseEvent(e);
	}

	protected virtual void OnDropDownOpening(CancelEventArgs e)
	{
		this.DropDownOpening?.Invoke(this, e);
	}

	protected virtual void OnDropDownOpened(EventArgs e)
	{
		this.DropDownOpened?.Invoke(this, e);
	}

	protected virtual void OnDropDownClosing(CancelEventArgs e)
	{
		this.DropDownClosing?.Invoke(this, e);
	}

	protected virtual void OnDropDownClosed(EventArgs e)
	{
		this.DropDownClosed?.Invoke(this, e);
	}

	protected virtual void OnTextChanged(TextChangedEventArgs e)
	{
		RaiseEvent(e);
	}

	private void ClosingDropDown(bool oldValue)
	{
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		OnDropDownClosing(cancelEventArgs);
		if (cancelEventArgs.Cancel)
		{
			_ignorePropertyChange = true;
			SetCurrentValue(IsDropDownOpenProperty, oldValue);
		}
		else
		{
			CloseDropDown();
		}
		UpdatePseudoClasses();
	}

	private void OpeningDropDown(bool oldValue)
	{
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		OnDropDownOpening(cancelEventArgs);
		if (cancelEventArgs.Cancel)
		{
			_ignorePropertyChange = true;
			SetCurrentValue(IsDropDownOpenProperty, oldValue);
		}
		else
		{
			OpenDropDown();
		}
		UpdatePseudoClasses();
	}

	private void DropDownPopup_Closed(object? sender, EventArgs e)
	{
		if (IsDropDownOpen)
		{
			SetCurrentValue(IsDropDownOpenProperty, value: false);
		}
		if (_popupHasOpened)
		{
			OnDropDownClosed(EventArgs.Empty);
		}
	}

	private void PopulateDropDown(object? sender, EventArgs e)
	{
		_delayTimer?.Stop();
		SearchText = Text;
		if (!TryPopulateAsync(SearchText))
		{
			PopulatingEventArgs populatingEventArgs = new PopulatingEventArgs(SearchText);
			OnPopulating(populatingEventArgs);
			if (!populatingEventArgs.Cancel)
			{
				PopulateComplete();
			}
		}
	}

	private bool TryPopulateAsync(string? searchText)
	{
		_populationCancellationTokenSource?.Cancel(throwOnFirstException: false);
		_populationCancellationTokenSource?.Dispose();
		_populationCancellationTokenSource = null;
		if (AsyncPopulator == null)
		{
			return false;
		}
		_populationCancellationTokenSource = new CancellationTokenSource();
		Task task = PopulateAsync(searchText, _populationCancellationTokenSource.Token);
		if (task.Status == TaskStatus.Created)
		{
			task.Start();
		}
		return true;
	}

	private async Task PopulateAsync(string? searchText, CancellationToken cancellationToken)
	{
		try
		{
			List<object> resultList = (await AsyncPopulator(searchText, cancellationToken)).ToList();
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			await Dispatcher.UIThread.InvokeAsync(delegate
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					SetCurrentValue(ItemsSourceProperty, resultList);
					PopulateComplete();
				}
			});
		}
		catch (TaskCanceledException)
		{
		}
		finally
		{
			_populationCancellationTokenSource?.Dispose();
			_populationCancellationTokenSource = null;
		}
	}

	private void OpenDropDown()
	{
		if (DropDownPopup != null)
		{
			DropDownPopup.IsOpen = true;
		}
		_popupHasOpened = true;
		OnDropDownOpened(EventArgs.Empty);
	}

	private void CloseDropDown()
	{
		if (_popupHasOpened)
		{
			if (SelectionAdapter != null)
			{
				SelectionAdapter.SelectedItem = null;
			}
			if (DropDownPopup != null)
			{
				DropDownPopup.IsOpen = false;
			}
			OnDropDownClosed(EventArgs.Empty);
		}
	}

	private string? FormatValue(object? value, bool clearDataContext)
	{
		string? result = FormatValue(value);
		if (clearDataContext && _valueBindingEvaluator != null)
		{
			_valueBindingEvaluator.ClearDataContext();
		}
		return result;
	}

	protected virtual string? FormatValue(object? value)
	{
		if (_valueBindingEvaluator != null)
		{
			return _valueBindingEvaluator.GetDynamicValue(value) ?? string.Empty;
		}
		if (value != null)
		{
			return value.ToString();
		}
		return string.Empty;
	}

	private void OnTextBoxTextChanged()
	{
		Dispatcher.UIThread.Post(delegate
		{
			TextUpdated(_textBox.Text, userInitiated: true);
		});
	}

	private void UpdateTextValue(string? value)
	{
		UpdateTextValue(value, null);
	}

	private void UpdateTextValue(string? value, bool? userInitiated)
	{
		bool flag = false;
		if ((userInitiated ?? true) && Text != value)
		{
			_ignoreTextPropertyChange++;
			SetCurrentValue(TextProperty, value);
			flag = true;
		}
		if ((!userInitiated.HasValue || userInitiated == false) && TextBox != null && TextBox.Text != value)
		{
			_ignoreTextPropertyChange++;
			TextBox.Text = value ?? string.Empty;
			if (!flag && (Text == value || Text == null))
			{
				flag = true;
			}
		}
		if (flag)
		{
			OnTextChanged(new TextChangedEventArgs(TextChangedEvent));
		}
	}

	private void TextUpdated(string? newText, bool userInitiated)
	{
		if (_ignoreTextPropertyChange > 0)
		{
			_ignoreTextPropertyChange--;
			return;
		}
		if (newText == null)
		{
			newText = string.Empty;
		}
		if (IsTextCompletionEnabled && TextBox != null && TextBoxSelectionLength > 0 && TextBoxSelectionStart != (TextBox.Text?.Length ?? 0))
		{
			return;
		}
		bool flag = newText.Length >= MinimumPrefixLength && MinimumPrefixLength >= 0;
		_userCalledPopulate = flag && userInitiated;
		UpdateTextValue(newText, userInitiated);
		if (flag)
		{
			_ignoreTextSelectionChange = true;
			if (_delayTimer != null)
			{
				_delayTimer.Start();
			}
			else
			{
				PopulateDropDown(this, EventArgs.Empty);
			}
			return;
		}
		SearchText = string.Empty;
		if (SelectedItem != null)
		{
			_skipSelectedItemTextUpdate = true;
		}
		SetCurrentValue(SelectedItemProperty, null);
		if (IsDropDownOpen)
		{
			SetCurrentValue(IsDropDownOpenProperty, value: false);
		}
	}

	private void ClearView()
	{
		if (_view == null)
		{
			_view = new AvaloniaList<object>();
		}
		else
		{
			_view.Clear();
		}
	}

	private void RefreshView()
	{
		if (_items == null)
		{
			ClearView();
			return;
		}
		string search = Text ?? string.Empty;
		bool flag = TextFilter != null;
		bool flag2 = FilterMode == AutoCompleteFilterMode.Custom && TextFilter == null;
		int num = 0;
		int num2 = _view.Count;
		foreach (object item in _items)
		{
			bool flag3 = !(flag || flag2);
			if (!flag3)
			{
				if (flag)
				{
					flag3 = TextFilter(search, FormatValue(item));
				}
				else
				{
					if (ItemFilter == null)
					{
						throw new Exception("ItemFilter property can not be null when FilterMode has value AutoCompleteFilterMode.Custom");
					}
					flag3 = ItemFilter(search, item);
				}
			}
			if (num2 > num && flag3 && _view[num] == item)
			{
				num++;
			}
			else if (flag3)
			{
				if (num2 > num && _view[num] != item)
				{
					_view.RemoveAt(num);
					_view.Insert(num, item);
					num++;
					continue;
				}
				if (num == num2)
				{
					_view.Add(item);
				}
				else
				{
					_view.Insert(num, item);
				}
				num++;
				num2++;
			}
			else if (num2 > num && _view[num] == item)
			{
				_view.RemoveAt(num);
				num2--;
			}
		}
		if (_valueBindingEvaluator != null)
		{
			_valueBindingEvaluator.ClearDataContext();
		}
	}

	private void OnItemsSourceChanged(IEnumerable? newValue)
	{
		_collectionChangeSubscription?.Dispose();
		_collectionChangeSubscription = null;
		if (newValue is INotifyCollectionChanged collection)
		{
			_collectionChangeSubscription = collection.WeakSubscribe(ItemsCollectionChanged);
		}
		_items = ((newValue == null) ? null : new List<object>(newValue.Cast<object>()));
		ClearView();
		if (SelectionAdapter != null && SelectionAdapter.ItemsSource != _view)
		{
			SelectionAdapter.ItemsSource = _view;
		}
		if (IsDropDownOpen)
		{
			RefreshView();
		}
	}

	private void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
		{
			for (int i = 0; i < e.OldItems.Count; i++)
			{
				_items.RemoveAt(e.OldStartingIndex);
			}
		}
		if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && _items.Count >= e.NewStartingIndex)
		{
			for (int j = 0; j < e.NewItems.Count; j++)
			{
				_items.Insert(e.NewStartingIndex + j, e.NewItems[j]);
			}
		}
		if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null && e.OldItems != null)
		{
			for (int k = 0; k < e.NewItems.Count; k++)
			{
				_items[e.NewStartingIndex] = e.NewItems[k];
			}
		}
		if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
		{
			for (int l = 0; l < e.OldItems.Count; l++)
			{
				_view.Remove(e.OldItems[l]);
			}
		}
		if (e.Action == NotifyCollectionChangedAction.Reset)
		{
			ClearView();
			if (ItemsSource != null)
			{
				_items = new List<object>(ItemsSource.Cast<object>());
			}
		}
		RefreshView();
	}

	public void PopulateComplete()
	{
		RefreshView();
		PopulatedEventArgs e = new PopulatedEventArgs(new ReadOnlyCollection<object>(_view));
		OnPopulated(e);
		if (SelectionAdapter != null && SelectionAdapter.ItemsSource != _view)
		{
			SelectionAdapter.ItemsSource = _view;
		}
		bool flag = _userCalledPopulate && _view.Count > 0;
		if (flag != IsDropDownOpen)
		{
			_ignorePropertyChange = true;
			SetCurrentValue(IsDropDownOpenProperty, flag);
		}
		if (IsDropDownOpen)
		{
			OpeningDropDown(oldValue: false);
		}
		else
		{
			ClosingDropDown(oldValue: true);
		}
		UpdateTextCompletion(_userCalledPopulate);
	}

	private void UpdateTextCompletion(bool userInitiated)
	{
		object obj = null;
		string text = Text;
		if (_view.Count > 0)
		{
			if (IsTextCompletionEnabled && TextBox != null && userInitiated)
			{
				int selectionStart = TextBox.Text?.Length ?? 0;
				int textBoxSelectionStart = TextBoxSelectionStart;
				if (textBoxSelectionStart == text?.Length && textBoxSelectionStart > _textSelectionStart)
				{
					object obj2 = ((FilterMode == AutoCompleteFilterMode.StartsWith || FilterMode == AutoCompleteFilterMode.StartsWithCaseSensitive) ? _view[0] : TryGetMatch(text, _view, AutoCompleteSearch.GetFilter(AutoCompleteFilterMode.StartsWith)));
					if (obj2 != null)
					{
						obj = obj2;
						string text2 = FormatValue(obj2, clearDataContext: true);
						int length = Math.Min(text2?.Length ?? 0, Text?.Length ?? 0);
						if (AutoCompleteSearch.Equals(Text?.Substring(0, length), text2?.Substring(0, length)))
						{
							UpdateTextValue(text2);
							TextBox.SelectionStart = selectionStart;
							TextBox.SelectionEnd = text2?.Length ?? 0;
						}
					}
				}
			}
			else
			{
				obj = TryGetMatch(text, _view, AutoCompleteSearch.GetFilter(AutoCompleteFilterMode.EqualsCaseSensitive));
			}
		}
		if (SelectedItem != obj)
		{
			_skipSelectedItemTextUpdate = true;
		}
		SetCurrentValue(SelectedItemProperty, obj);
		if (_ignoreTextSelectionChange)
		{
			_ignoreTextSelectionChange = false;
			if (TextBox != null)
			{
				_textSelectionStart = TextBoxSelectionStart;
			}
		}
	}

	private object? TryGetMatch(string? searchText, AvaloniaList<object>? view, AutoCompleteFilterPredicate<string?>? predicate)
	{
		if (predicate == null)
		{
			return null;
		}
		if (view != null && view.Count > 0)
		{
			foreach (object item in view)
			{
				if (predicate(searchText, FormatValue(item)))
				{
					return item;
				}
			}
		}
		return null;
	}

	private void UpdatePseudoClasses()
	{
		base.PseudoClasses.Set(":dropdownopen", IsDropDownOpen);
	}

	private void ClearTextBoxSelection()
	{
		if (TextBox != null)
		{
			int num = TextBox.Text?.Length ?? 0;
			TextBox.SelectionStart = num;
			TextBox.SelectionEnd = num;
		}
	}

	private void OnSelectedItemChanged(object? newItem)
	{
		string value = ((newItem == null) ? SearchText : ((TextSelector != null) ? TextSelector(SearchText, FormatValue(newItem, clearDataContext: true)) : ((ItemSelector == null) ? FormatValue(newItem, clearDataContext: true) : ItemSelector(SearchText, newItem))));
		UpdateTextValue(value);
		ClearTextBoxSelection();
	}

	private void OnAdapterSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		SetCurrentValue(SelectedItemProperty, _adapter.SelectedItem);
	}

	private void OnAdapterSelectionComplete(object? sender, RoutedEventArgs e)
	{
		SetCurrentValue(IsDropDownOpenProperty, value: false);
		ClearTextBoxSelection();
		TextBox.Focus();
	}

	private void OnAdapterSelectionCanceled(object? sender, RoutedEventArgs e)
	{
		UpdateTextValue(SearchText);
		UpdateTextCompletion(userInitiated: false);
	}
}
