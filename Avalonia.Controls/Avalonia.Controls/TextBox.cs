using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Utils;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Controls;

[TemplatePart("PART_TextPresenter", typeof(TextPresenter))]
[TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
[PseudoClasses(new string[] { ":empty" })]
public class TextBox : TemplatedControl, UndoRedoHelper<TextBox.UndoRedoState>.IUndoRedoHost
{
	private readonly struct UndoRedoState : IEquatable<UndoRedoState>
	{
		public string? Text { get; }

		public int CaretPosition { get; }

		public UndoRedoState(string? text, int caretPosition)
		{
			Text = text;
			CaretPosition = caretPosition;
		}

		public bool Equals(UndoRedoState other)
		{
			if ((object)Text != other.Text)
			{
				return object.Equals(Text, other.Text);
			}
			return true;
		}

		public override bool Equals(object? obj)
		{
			if (obj is UndoRedoState other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Text?.GetHashCode() ?? 0;
		}
	}

	private class MaxLinesTextSource : ITextSource
	{
		private readonly int _maxLines;

		public MaxLinesTextSource(int maxLines)
		{
			_maxLines = maxLines;
		}

		public TextRun? GetTextRun(int textSourceIndex)
		{
			if (textSourceIndex >= _maxLines)
			{
				return null;
			}
			return new TextEndOfLine();
		}
	}

	public static readonly StyledProperty<bool> AcceptsReturnProperty;

	public static readonly StyledProperty<bool> AcceptsTabProperty;

	public static readonly StyledProperty<int> CaretIndexProperty;

	public static readonly StyledProperty<bool> IsReadOnlyProperty;

	public static readonly StyledProperty<char> PasswordCharProperty;

	public static readonly StyledProperty<IBrush?> SelectionBrushProperty;

	public static readonly StyledProperty<IBrush?> SelectionForegroundBrushProperty;

	public static readonly StyledProperty<IBrush?> CaretBrushProperty;

	public static readonly StyledProperty<int> SelectionStartProperty;

	public static readonly StyledProperty<int> SelectionEndProperty;

	public static readonly StyledProperty<int> MaxLengthProperty;

	public static readonly StyledProperty<int> MaxLinesProperty;

	public static readonly StyledProperty<string?> TextProperty;

	public static readonly StyledProperty<TextAlignment> TextAlignmentProperty;

	public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty;

	public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty;

	public static readonly StyledProperty<TextWrapping> TextWrappingProperty;

	public static readonly StyledProperty<double> LineHeightProperty;

	public static readonly StyledProperty<double> LetterSpacingProperty;

	public static readonly StyledProperty<string?> WatermarkProperty;

	public static readonly StyledProperty<bool> UseFloatingWatermarkProperty;

	public static readonly StyledProperty<string> NewLineProperty;

	public static readonly StyledProperty<object> InnerLeftContentProperty;

	public static readonly StyledProperty<object> InnerRightContentProperty;

	public static readonly StyledProperty<bool> RevealPasswordProperty;

	public static readonly DirectProperty<TextBox, bool> CanCutProperty;

	public static readonly DirectProperty<TextBox, bool> CanCopyProperty;

	public static readonly DirectProperty<TextBox, bool> CanPasteProperty;

	public static readonly StyledProperty<bool> IsUndoEnabledProperty;

	public static readonly StyledProperty<int> UndoLimitProperty;

	public static readonly DirectProperty<TextBox, bool> CanUndoProperty;

	public static readonly DirectProperty<TextBox, bool> CanRedoProperty;

	public static readonly RoutedEvent<RoutedEventArgs> CopyingToClipboardEvent;

	public static readonly RoutedEvent<RoutedEventArgs> CuttingToClipboardEvent;

	public static readonly RoutedEvent<RoutedEventArgs> PastingFromClipboardEvent;

	public static readonly RoutedEvent<TextChangedEventArgs> TextChangedEvent;

	public static readonly RoutedEvent<TextChangingEventArgs> TextChangingEvent;

	private TextPresenter? _presenter;

	private ScrollViewer? _scrollViewer;

	private readonly TextBoxTextInputMethodClient _imClient = new TextBoxTextInputMethodClient();

	private readonly UndoRedoHelper<UndoRedoState> _undoRedoHelper;

	private bool _isUndoingRedoing;

	private bool _canCut;

	private bool _canCopy;

	private bool _canPaste;

	private static readonly string[] invalidCharacters;

	private bool _canUndo;

	private bool _canRedo;

	private int _wordSelectionStart = -1;

	private int _selectedTextChangesMadeSinceLastUndoSnapshot;

	private bool _hasDoneSnapshotOnce;

	private const int _maxCharsBeforeUndoSnapshot = 7;

	public static KeyGesture? CutGesture => Application.Current?.PlatformSettings?.HotkeyConfiguration.Cut.FirstOrDefault();

	public static KeyGesture? CopyGesture => Application.Current?.PlatformSettings?.HotkeyConfiguration.Copy.FirstOrDefault();

	public static KeyGesture? PasteGesture => Application.Current?.PlatformSettings?.HotkeyConfiguration.Paste.FirstOrDefault();

	public bool AcceptsReturn
	{
		get
		{
			return GetValue(AcceptsReturnProperty);
		}
		set
		{
			SetValue(AcceptsReturnProperty, value);
		}
	}

	public bool AcceptsTab
	{
		get
		{
			return GetValue(AcceptsTabProperty);
		}
		set
		{
			SetValue(AcceptsTabProperty, value);
		}
	}

	public int CaretIndex
	{
		get
		{
			return GetValue(CaretIndexProperty);
		}
		set
		{
			SetValue(CaretIndexProperty, value);
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return GetValue(IsReadOnlyProperty);
		}
		set
		{
			SetValue(IsReadOnlyProperty, value);
		}
	}

	public char PasswordChar
	{
		get
		{
			return GetValue(PasswordCharProperty);
		}
		set
		{
			SetValue(PasswordCharProperty, value);
		}
	}

	public IBrush? SelectionBrush
	{
		get
		{
			return GetValue(SelectionBrushProperty);
		}
		set
		{
			SetValue(SelectionBrushProperty, value);
		}
	}

	public IBrush? SelectionForegroundBrush
	{
		get
		{
			return GetValue(SelectionForegroundBrushProperty);
		}
		set
		{
			SetValue(SelectionForegroundBrushProperty, value);
		}
	}

	public IBrush? CaretBrush
	{
		get
		{
			return GetValue(CaretBrushProperty);
		}
		set
		{
			SetValue(CaretBrushProperty, value);
		}
	}

	public int SelectionStart
	{
		get
		{
			return GetValue(SelectionStartProperty);
		}
		set
		{
			SetValue(SelectionStartProperty, value);
		}
	}

	public int SelectionEnd
	{
		get
		{
			return GetValue(SelectionEndProperty);
		}
		set
		{
			SetValue(SelectionEndProperty, value);
		}
	}

	public int MaxLength
	{
		get
		{
			return GetValue(MaxLengthProperty);
		}
		set
		{
			SetValue(MaxLengthProperty, value);
		}
	}

	public int MaxLines
	{
		get
		{
			return GetValue(MaxLinesProperty);
		}
		set
		{
			SetValue(MaxLinesProperty, value);
		}
	}

	public double LetterSpacing
	{
		get
		{
			return GetValue(LetterSpacingProperty);
		}
		set
		{
			SetValue(LetterSpacingProperty, value);
		}
	}

	public double LineHeight
	{
		get
		{
			return GetValue(LineHeightProperty);
		}
		set
		{
			SetValue(LineHeightProperty, value);
		}
	}

	[Content]
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

	public string SelectedText
	{
		get
		{
			return GetSelection();
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				_selectedTextChangesMadeSinceLastUndoSnapshot++;
				SnapshotUndoRedo(ignoreChangeCount: false);
				DeleteSelection();
			}
			else
			{
				HandleTextInput(value);
			}
		}
	}

	public HorizontalAlignment HorizontalContentAlignment
	{
		get
		{
			return GetValue(HorizontalContentAlignmentProperty);
		}
		set
		{
			SetValue(HorizontalContentAlignmentProperty, value);
		}
	}

	public VerticalAlignment VerticalContentAlignment
	{
		get
		{
			return GetValue(VerticalContentAlignmentProperty);
		}
		set
		{
			SetValue(VerticalContentAlignmentProperty, value);
		}
	}

	public TextAlignment TextAlignment
	{
		get
		{
			return GetValue(TextAlignmentProperty);
		}
		set
		{
			SetValue(TextAlignmentProperty, value);
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

	public bool UseFloatingWatermark
	{
		get
		{
			return GetValue(UseFloatingWatermarkProperty);
		}
		set
		{
			SetValue(UseFloatingWatermarkProperty, value);
		}
	}

	public object InnerLeftContent
	{
		get
		{
			return GetValue(InnerLeftContentProperty);
		}
		set
		{
			SetValue(InnerLeftContentProperty, value);
		}
	}

	public object InnerRightContent
	{
		get
		{
			return GetValue(InnerRightContentProperty);
		}
		set
		{
			SetValue(InnerRightContentProperty, value);
		}
	}

	public bool RevealPassword
	{
		get
		{
			return GetValue(RevealPasswordProperty);
		}
		set
		{
			SetValue(RevealPasswordProperty, value);
		}
	}

	public TextWrapping TextWrapping
	{
		get
		{
			return GetValue(TextWrappingProperty);
		}
		set
		{
			SetValue(TextWrappingProperty, value);
		}
	}

	public string NewLine
	{
		get
		{
			return GetValue(NewLineProperty);
		}
		set
		{
			SetValue(NewLineProperty, value);
		}
	}

	public bool CanCut
	{
		get
		{
			return _canCut;
		}
		private set
		{
			SetAndRaise(CanCutProperty, ref _canCut, value);
		}
	}

	public bool CanCopy
	{
		get
		{
			return _canCopy;
		}
		private set
		{
			SetAndRaise(CanCopyProperty, ref _canCopy, value);
		}
	}

	public bool CanPaste
	{
		get
		{
			return _canPaste;
		}
		private set
		{
			SetAndRaise(CanPasteProperty, ref _canPaste, value);
		}
	}

	public bool IsUndoEnabled
	{
		get
		{
			return GetValue(IsUndoEnabledProperty);
		}
		set
		{
			SetValue(IsUndoEnabledProperty, value);
		}
	}

	public int UndoLimit
	{
		get
		{
			return GetValue(UndoLimitProperty);
		}
		set
		{
			SetValue(UndoLimitProperty, value);
		}
	}

	public bool CanUndo
	{
		get
		{
			return _canUndo;
		}
		private set
		{
			SetAndRaise(CanUndoProperty, ref _canUndo, value);
		}
	}

	public bool CanRedo
	{
		get
		{
			return _canRedo;
		}
		private set
		{
			SetAndRaise(CanRedoProperty, ref _canRedo, value);
		}
	}

	private bool IsPasswordBox => PasswordChar != '\0';

	UndoRedoState UndoRedoHelper<UndoRedoState>.IUndoRedoHost.UndoRedoState
	{
		get
		{
			return new UndoRedoState(Text, CaretIndex);
		}
		set
		{
			SetCurrentValue(TextProperty, value.Text);
			SetCurrentValue(CaretIndexProperty, value.CaretPosition);
			ClearSelection();
		}
	}

	public event EventHandler<RoutedEventArgs>? CopyingToClipboard
	{
		add
		{
			AddHandler(CopyingToClipboardEvent, value);
		}
		remove
		{
			RemoveHandler(CopyingToClipboardEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? CuttingToClipboard
	{
		add
		{
			AddHandler(CuttingToClipboardEvent, value);
		}
		remove
		{
			RemoveHandler(CuttingToClipboardEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? PastingFromClipboard
	{
		add
		{
			AddHandler(PastingFromClipboardEvent, value);
		}
		remove
		{
			RemoveHandler(PastingFromClipboardEvent, value);
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

	public event EventHandler<TextChangingEventArgs>? TextChanging
	{
		add
		{
			AddHandler(TextChangingEvent, value);
		}
		remove
		{
			RemoveHandler(TextChangingEvent, value);
		}
	}

	static TextBox()
	{
		AcceptsReturnProperty = AvaloniaProperty.Register<TextBox, bool>("AcceptsReturn", defaultValue: false);
		AcceptsTabProperty = AvaloniaProperty.Register<TextBox, bool>("AcceptsTab", defaultValue: false);
		CaretIndexProperty = AvaloniaProperty.Register<TextBox, int>("CaretIndex", 0, inherits: false, BindingMode.OneWay, null, CoerceCaretIndex);
		IsReadOnlyProperty = AvaloniaProperty.Register<TextBox, bool>("IsReadOnly", defaultValue: false);
		PasswordCharProperty = AvaloniaProperty.Register<TextBox, char>("PasswordChar", '\0');
		SelectionBrushProperty = AvaloniaProperty.Register<TextBox, IBrush>("SelectionBrush");
		SelectionForegroundBrushProperty = AvaloniaProperty.Register<TextBox, IBrush>("SelectionForegroundBrush");
		CaretBrushProperty = AvaloniaProperty.Register<TextBox, IBrush>("CaretBrush");
		SelectionStartProperty = AvaloniaProperty.Register<TextBox, int>("SelectionStart", 0, inherits: false, BindingMode.OneWay, null, CoerceCaretIndex);
		SelectionEndProperty = AvaloniaProperty.Register<TextBox, int>("SelectionEnd", 0, inherits: false, BindingMode.OneWay, null, CoerceCaretIndex);
		MaxLengthProperty = AvaloniaProperty.Register<TextBox, int>("MaxLength", 0);
		MaxLinesProperty = AvaloniaProperty.Register<TextBox, int>("MaxLines", 0);
		StyledProperty<string?> textProperty = TextBlock.TextProperty;
		Func<AvaloniaObject, string, string> coerce = CoerceText;
		TextProperty = textProperty.AddOwner<TextBox>(new StyledPropertyMetadata<string>(default(Optional<string>), BindingMode.TwoWay, coerce, enableDataValidation: true));
		TextAlignmentProperty = TextBlock.TextAlignmentProperty.AddOwner<TextBox>();
		HorizontalContentAlignmentProperty = ContentControl.HorizontalContentAlignmentProperty.AddOwner<TextBox>();
		VerticalContentAlignmentProperty = ContentControl.VerticalContentAlignmentProperty.AddOwner<TextBox>();
		TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner<TextBox>();
		LineHeightProperty = TextBlock.LineHeightProperty.AddOwner<TextBox>(new StyledPropertyMetadata<double>(double.NaN));
		LetterSpacingProperty = TextBlock.LetterSpacingProperty.AddOwner<TextBox>();
		WatermarkProperty = AvaloniaProperty.Register<TextBox, string>("Watermark");
		UseFloatingWatermarkProperty = AvaloniaProperty.Register<TextBox, bool>("UseFloatingWatermark", defaultValue: false);
		NewLineProperty = AvaloniaProperty.Register<TextBox, string>("NewLine", Environment.NewLine);
		InnerLeftContentProperty = AvaloniaProperty.Register<TextBox, object>("InnerLeftContent");
		InnerRightContentProperty = AvaloniaProperty.Register<TextBox, object>("InnerRightContent");
		RevealPasswordProperty = AvaloniaProperty.Register<TextBox, bool>("RevealPassword", defaultValue: false);
		CanCutProperty = AvaloniaProperty.RegisterDirect("CanCut", (TextBox o) => o.CanCut, null, unsetValue: false);
		CanCopyProperty = AvaloniaProperty.RegisterDirect("CanCopy", (TextBox o) => o.CanCopy, null, unsetValue: false);
		CanPasteProperty = AvaloniaProperty.RegisterDirect("CanPaste", (TextBox o) => o.CanPaste, null, unsetValue: false);
		IsUndoEnabledProperty = AvaloniaProperty.Register<TextBox, bool>("IsUndoEnabled", defaultValue: true);
		UndoLimitProperty = AvaloniaProperty.Register<TextBox, int>("UndoLimit", 10);
		CanUndoProperty = AvaloniaProperty.RegisterDirect("CanUndo", (TextBox x) => x.CanUndo, null, unsetValue: false);
		CanRedoProperty = AvaloniaProperty.RegisterDirect("CanRedo", (TextBox x) => x.CanRedo, null, unsetValue: false);
		CopyingToClipboardEvent = RoutedEvent.Register<TextBox, RoutedEventArgs>("CopyingToClipboard", RoutingStrategies.Bubble);
		CuttingToClipboardEvent = RoutedEvent.Register<TextBox, RoutedEventArgs>("CuttingToClipboard", RoutingStrategies.Bubble);
		PastingFromClipboardEvent = RoutedEvent.Register<TextBox, RoutedEventArgs>("PastingFromClipboard", RoutingStrategies.Bubble);
		TextChangedEvent = RoutedEvent.Register<TextBox, TextChangedEventArgs>("TextChanged", RoutingStrategies.Bubble);
		TextChangingEvent = RoutedEvent.Register<TextBox, TextChangingEventArgs>("TextChanging", RoutingStrategies.Bubble);
		invalidCharacters = new string[1] { "\u007f" };
		InputElement.FocusableProperty.OverrideDefaultValue(typeof(TextBox), defaultValue: true);
		InputElement.TextInputMethodClientRequestedEvent.AddClassHandler(delegate(TextBox tb, TextInputMethodClientRequestedEventArgs e)
		{
			if (!tb.IsReadOnly)
			{
				e.Client = tb._imClient;
			}
		});
	}

	public TextBox()
	{
		IObservable<ScrollBarVisibility> source = this.GetObservable(AcceptsReturnProperty).CombineLatest(this.GetObservable(TextWrappingProperty), delegate(bool acceptsReturn, TextWrapping wrapping)
		{
			if (wrapping != 0)
			{
				return ScrollBarVisibility.Disabled;
			}
			return acceptsReturn ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
		});
		Bind(ScrollViewer.HorizontalScrollBarVisibilityProperty, source, BindingPriority.Style);
		_undoRedoHelper = new UndoRedoHelper<UndoRedoState>(this);
		_selectedTextChangesMadeSinceLastUndoSnapshot = 0;
		_hasDoneSnapshotOnce = false;
		UpdatePseudoclasses();
	}

	private void OnCaretIndexChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (IsUndoEnabled && _undoRedoHelper.TryGetLastState(out var _state) && _state.Text == Text)
		{
			_undoRedoHelper.UpdateLastState();
		}
		int newValue = e.GetNewValue<int>();
		SetCurrentValue(SelectionStartProperty, newValue);
		SetCurrentValue(SelectionEndProperty, newValue);
	}

	private void OnSelectionStartChanged(AvaloniaPropertyChangedEventArgs e)
	{
		UpdateCommandStates();
		int newValue = e.GetNewValue<int>();
		if (SelectionEnd == newValue && CaretIndex != newValue)
		{
			SetCurrentValue(CaretIndexProperty, newValue);
		}
	}

	private void OnSelectionEndChanged(AvaloniaPropertyChangedEventArgs e)
	{
		UpdateCommandStates();
		int newValue = e.GetNewValue<int>();
		if (SelectionStart == newValue && CaretIndex != newValue)
		{
			SetCurrentValue(CaretIndexProperty, newValue);
		}
	}

	private static string? CoerceText(AvaloniaObject sender, string? value)
	{
		TextBox textBox = (TextBox)sender;
		if (!textBox._isUndoingRedoing)
		{
			textBox.SnapshotUndoRedo();
		}
		return value;
	}

	public void ClearSelection()
	{
		SetCurrentValue(CaretIndexProperty, SelectionStart);
		SetCurrentValue(SelectionEndProperty, SelectionStart);
	}

	private void OnUndoLimitChanged(int newValue)
	{
		_undoRedoHelper.Limit = newValue;
		_undoRedoHelper.Clear();
		_selectedTextChangesMadeSinceLastUndoSnapshot = 0;
		_hasDoneSnapshotOnce = false;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_presenter = e.NameScope.Get<TextPresenter>("PART_TextPresenter");
		_scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
		_imClient.SetPresenter(_presenter, this);
		if (base.IsFocused)
		{
			_presenter?.ShowCaret();
		}
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		if (_presenter != null)
		{
			if (base.IsFocused)
			{
				_presenter.ShowCaret();
			}
			_presenter.PropertyChanged += PresenterPropertyChanged;
		}
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		if (_presenter != null)
		{
			_presenter.HideCaret();
			_presenter.PropertyChanged -= PresenterPropertyChanged;
		}
		_imClient.SetPresenter(null, null);
	}

	private void PresenterPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == TextPresenter.PreeditTextProperty && string.IsNullOrEmpty(e.OldValue as string) && !string.IsNullOrEmpty(e.NewValue as string))
		{
			base.PseudoClasses.Set(":empty", value: false);
			DeleteSelection();
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == TextProperty)
		{
			CoerceValue(CaretIndexProperty);
			CoerceValue(SelectionStartProperty);
			CoerceValue(SelectionEndProperty);
			RaiseTextChangeEvents();
			UpdatePseudoclasses();
			UpdateCommandStates();
		}
		else if (change.Property == CaretIndexProperty)
		{
			OnCaretIndexChanged(change);
		}
		else if (change.Property == SelectionStartProperty)
		{
			OnSelectionStartChanged(change);
		}
		else if (change.Property == SelectionEndProperty)
		{
			OnSelectionEndChanged(change);
		}
		else if (change.Property == MaxLinesProperty)
		{
			InvalidateMeasure();
		}
		else if (change.Property == UndoLimitProperty)
		{
			OnUndoLimitChanged(change.GetNewValue<int>());
		}
		else if (change.Property == IsUndoEnabledProperty && !change.GetNewValue<bool>())
		{
			_undoRedoHelper.Clear();
			_selectedTextChangesMadeSinceLastUndoSnapshot = 0;
			_hasDoneSnapshotOnce = false;
		}
	}

	private void UpdateCommandStates()
	{
		bool flag = string.IsNullOrEmpty(GetSelection());
		CanCopy = !IsPasswordBox && !flag;
		CanCut = !IsPasswordBox && !flag && !IsReadOnly;
		CanPaste = !IsReadOnly;
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		if (e.NavigationMethod == NavigationMethod.Tab && !AcceptsReturn)
		{
			string? text = Text;
			if (text != null && text.Length > 0)
			{
				SelectAll();
			}
		}
		UpdateCommandStates();
		_imClient.SetPresenter(_presenter, this);
		_presenter?.ShowCaret();
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		if ((base.ContextFlyout == null || !base.ContextFlyout.IsOpen) && (base.ContextMenu == null || !base.ContextMenu.IsOpen))
		{
			ClearSelection();
			SetCurrentValue(RevealPasswordProperty, value: false);
		}
		UpdateCommandStates();
		_presenter?.HideCaret();
		_imClient.SetPresenter(null, null);
	}

	protected override void OnTextInput(TextInputEventArgs e)
	{
		if (!e.Handled)
		{
			HandleTextInput(e.Text);
			e.Handled = true;
		}
	}

	private void HandleTextInput(string? input)
	{
		if (IsReadOnly)
		{
			return;
		}
		input = RemoveInvalidCharacters(input);
		if (string.IsNullOrEmpty(input))
		{
			return;
		}
		_selectedTextChangesMadeSinceLastUndoSnapshot++;
		SnapshotUndoRedo(ignoreChangeCount: false);
		string text = Text ?? string.Empty;
		int num = Math.Abs(SelectionStart - SelectionEnd);
		int num2 = input.Length + text.Length - num;
		if (MaxLength > 0 && num2 > MaxLength)
		{
			input = input.Remove(Math.Max(0, input.Length - (num2 - MaxLength)));
			num2 = MaxLength;
		}
		if (!string.IsNullOrEmpty(input))
		{
			StringBuilder stringBuilder = StringBuilderCache.Acquire(Math.Max(text.Length, num2));
			stringBuilder.Append(text);
			int num3 = CaretIndex;
			if (num != 0)
			{
				int item = GetSelectionRange().start;
				stringBuilder.Remove(item, num);
				num3 = item;
			}
			stringBuilder.Insert(num3, input);
			string stringAndRelease = StringBuilderCache.GetStringAndRelease(stringBuilder);
			SetCurrentValue(TextProperty, stringAndRelease);
			ClearSelection();
			if (IsUndoEnabled)
			{
				_undoRedoHelper.DiscardRedo();
			}
			SetCurrentValue(CaretIndexProperty, num3 + input.Length);
		}
	}

	private string? RemoveInvalidCharacters(string? text)
	{
		if (text == null)
		{
			return null;
		}
		for (int i = 0; i < invalidCharacters.Length; i++)
		{
			text = text.Replace(invalidCharacters[i], string.Empty);
		}
		return text;
	}

	public async void Cut()
	{
		string selection = GetSelection();
		if (string.IsNullOrEmpty(selection))
		{
			return;
		}
		RoutedEventArgs routedEventArgs = new RoutedEventArgs(CuttingToClipboardEvent);
		RaiseEvent(routedEventArgs);
		if (!routedEventArgs.Handled)
		{
			SnapshotUndoRedo();
			IClipboard clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
			if (clipboard != null)
			{
				await clipboard.SetTextAsync(selection);
				DeleteSelection();
			}
		}
	}

	public async void Copy()
	{
		string selection = GetSelection();
		if (string.IsNullOrEmpty(selection))
		{
			return;
		}
		RoutedEventArgs routedEventArgs = new RoutedEventArgs(CopyingToClipboardEvent);
		RaiseEvent(routedEventArgs);
		if (!routedEventArgs.Handled)
		{
			IClipboard clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
			if (clipboard != null)
			{
				await clipboard.SetTextAsync(selection);
			}
		}
	}

	public async void Paste()
	{
		RoutedEventArgs routedEventArgs = new RoutedEventArgs(PastingFromClipboardEvent);
		RaiseEvent(routedEventArgs);
		if (!routedEventArgs.Handled)
		{
			string text = null;
			IClipboard clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
			if (clipboard != null)
			{
				text = await clipboard.GetTextAsync();
			}
			if (!string.IsNullOrEmpty(text))
			{
				SnapshotUndoRedo();
				HandleTextInput(text);
			}
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (_presenter == null || !string.IsNullOrEmpty(_presenter.PreeditText))
		{
			return;
		}
		string text = Text ?? string.Empty;
		int caretIndex = CaretIndex;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		KeyModifiers keyModifiers = e.KeyModifiers;
		PlatformHotkeyConfiguration keymap = Application.Current.PlatformSettings.HotkeyConfiguration;
		if (Match(keymap.SelectAll))
		{
			SelectAll();
			flag3 = true;
		}
		else if (Match(keymap.Copy))
		{
			if (!IsPasswordBox)
			{
				Copy();
			}
			flag3 = true;
		}
		else if (Match(keymap.Cut))
		{
			if (!IsPasswordBox)
			{
				Cut();
			}
			flag3 = true;
		}
		else if (Match(keymap.Paste))
		{
			Paste();
			flag3 = true;
		}
		else if (Match(keymap.Undo) && IsUndoEnabled)
		{
			Undo();
			flag3 = true;
		}
		else if (Match(keymap.Redo) && IsUndoEnabled)
		{
			Redo();
			flag3 = true;
		}
		else if (Match(keymap.MoveCursorToTheStartOfDocument))
		{
			MoveHome(document: true);
			flag = true;
			flag2 = false;
			flag3 = true;
			SetCurrentValue(CaretIndexProperty, _presenter.CaretIndex);
		}
		else if (Match(keymap.MoveCursorToTheEndOfDocument))
		{
			MoveEnd(document: true);
			flag = true;
			flag2 = false;
			flag3 = true;
			SetCurrentValue(CaretIndexProperty, _presenter.CaretIndex);
		}
		else if (Match(keymap.MoveCursorToTheStartOfLine))
		{
			MoveHome(document: false);
			flag = true;
			flag2 = false;
			flag3 = true;
			SetCurrentValue(CaretIndexProperty, _presenter.CaretIndex);
		}
		else if (Match(keymap.MoveCursorToTheEndOfLine))
		{
			MoveEnd(document: false);
			flag = true;
			flag2 = false;
			flag3 = true;
			SetCurrentValue(CaretIndexProperty, _presenter.CaretIndex);
		}
		else if (Match(keymap.MoveCursorToTheStartOfDocumentWithSelection))
		{
			SetCurrentValue(SelectionStartProperty, caretIndex);
			MoveHome(document: true);
			SetCurrentValue(SelectionEndProperty, _presenter.CaretIndex);
			flag = true;
			flag2 = true;
			flag3 = true;
		}
		else if (Match(keymap.MoveCursorToTheEndOfDocumentWithSelection))
		{
			SetCurrentValue(SelectionStartProperty, caretIndex);
			MoveEnd(document: true);
			SetCurrentValue(SelectionEndProperty, _presenter.CaretIndex);
			flag = true;
			flag2 = true;
			flag3 = true;
		}
		else if (Match(keymap.MoveCursorToTheStartOfLineWithSelection))
		{
			SetCurrentValue(SelectionStartProperty, caretIndex);
			MoveHome(document: false);
			SetCurrentValue(SelectionEndProperty, _presenter.CaretIndex);
			flag = true;
			flag2 = true;
			flag3 = true;
		}
		else if (Match(keymap.MoveCursorToTheEndOfLineWithSelection))
		{
			SetCurrentValue(SelectionStartProperty, caretIndex);
			MoveEnd(document: false);
			SetCurrentValue(SelectionEndProperty, _presenter.CaretIndex);
			flag = true;
			flag2 = true;
			flag3 = true;
		}
		else if (Match(keymap.PageLeft))
		{
			MovePageLeft();
			flag = true;
			flag2 = false;
			flag3 = true;
		}
		else if (Match(keymap.PageRight))
		{
			MovePageRight();
			flag = true;
			flag2 = false;
			flag3 = true;
		}
		else if (Match(keymap.PageUp))
		{
			MovePageUp();
			flag = true;
			flag2 = false;
			flag3 = true;
		}
		else if (Match(keymap.PageDown))
		{
			MovePageDown();
			flag = true;
			flag2 = false;
			flag3 = true;
		}
		else
		{
			bool flag4 = keyModifiers.HasAllFlags(keymap.WholeWordTextActionModifiers);
			switch (e.Key)
			{
			case Key.Left:
				flag2 = DetectSelection();
				MoveHorizontal(-1, flag4, flag2);
				flag = true;
				break;
			case Key.Right:
				flag2 = DetectSelection();
				MoveHorizontal(1, flag4, flag2);
				flag = true;
				break;
			case Key.Up:
				flag2 = DetectSelection();
				_presenter.MoveCaretVertical(LogicalDirection.Backward);
				if (caretIndex != _presenter.CaretIndex)
				{
					flag = true;
				}
				if (flag2)
				{
					SetCurrentValue(SelectionEndProperty, _presenter.CaretIndex);
				}
				else
				{
					SetCurrentValue(CaretIndexProperty, _presenter.CaretIndex);
				}
				break;
			case Key.Down:
				flag2 = DetectSelection();
				_presenter.MoveCaretVertical();
				if (caretIndex != _presenter.CaretIndex)
				{
					flag = true;
				}
				if (flag2)
				{
					SetCurrentValue(SelectionEndProperty, _presenter.CaretIndex);
				}
				else
				{
					SetCurrentValue(CaretIndexProperty, _presenter.CaretIndex);
				}
				break;
			case Key.Back:
				SnapshotUndoRedo();
				if (flag4 && SelectionStart == SelectionEnd)
				{
					SetSelectionForControlBackspace();
				}
				if (!DeleteSelection())
				{
					CharacterHit nextCharacterHit2 = _presenter.GetNextCharacterHit(LogicalDirection.Backward);
					int num4 = nextCharacterHit2.FirstCharacterIndex + nextCharacterHit2.TrailingLength;
					if (caretIndex != num4)
					{
						int num5 = Math.Min(num4, caretIndex);
						int num6 = Math.Max(num4, caretIndex);
						StringBuilder stringBuilder2 = StringBuilderCache.Acquire(text.Length);
						stringBuilder2.Append(text);
						stringBuilder2.Remove(num5, num6 - num5);
						SetCurrentValue(TextProperty, StringBuilderCache.GetStringAndRelease(stringBuilder2));
						SetCurrentValue(CaretIndexProperty, num5);
					}
				}
				SnapshotUndoRedo();
				flag3 = true;
				break;
			case Key.Delete:
				SnapshotUndoRedo();
				if (flag4 && SelectionStart == SelectionEnd)
				{
					SetSelectionForControlDelete();
				}
				if (!DeleteSelection())
				{
					CharacterHit nextCharacterHit = _presenter.GetNextCharacterHit();
					int num = nextCharacterHit.FirstCharacterIndex + nextCharacterHit.TrailingLength;
					if (num != caretIndex)
					{
						int num2 = Math.Min(num, caretIndex);
						int num3 = Math.Max(num, caretIndex);
						StringBuilder stringBuilder = StringBuilderCache.Acquire(text.Length);
						stringBuilder.Append(text);
						stringBuilder.Remove(num2, num3 - num2);
						SetCurrentValue(TextProperty, StringBuilderCache.GetStringAndRelease(stringBuilder));
					}
				}
				SnapshotUndoRedo();
				flag3 = true;
				break;
			case Key.Return:
				if (AcceptsReturn)
				{
					SnapshotUndoRedo();
					HandleTextInput(NewLine);
					flag3 = true;
				}
				break;
			case Key.Tab:
				if (AcceptsTab)
				{
					SnapshotUndoRedo();
					HandleTextInput("\t");
					flag3 = true;
				}
				else
				{
					base.OnKeyDown(e);
				}
				break;
			case Key.Space:
				SnapshotUndoRedo();
				break;
			default:
				flag3 = false;
				break;
			}
		}
		if (flag && !flag2)
		{
			ClearSelection();
		}
		if (flag3 || flag)
		{
			e.Handled = true;
		}
		bool DetectSelection()
		{
			return e.KeyModifiers.HasAllFlags(keymap.SelectionModifiers);
		}
		bool Match(List<KeyGesture> gestures)
		{
			return gestures.Any((KeyGesture g) => g.Matches(e));
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		if (_presenter == null)
		{
			return;
		}
		string text = Text;
		PointerPoint currentPoint = e.GetCurrentPoint(this);
		if (text != null && currentPoint.Properties.IsLeftButtonPressed && !(currentPoint.Pointer?.Captured is Border))
		{
			Point position = e.GetPosition(_presenter);
			_presenter.MoveCaretToPoint(position);
			int caretIndex = _presenter.CaretIndex;
			bool flag = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
			int selectionStart = SelectionStart;
			int selectionEnd = SelectionEnd;
			switch (e.ClickCount)
			{
			case 1:
				if (flag)
				{
					if (_wordSelectionStart >= 0)
					{
						UpdateWordSelectionRange(caretIndex, ref selectionStart, ref selectionEnd);
						SetCurrentValue(SelectionStartProperty, selectionStart);
						SetCurrentValue(SelectionEndProperty, selectionEnd);
					}
					else
					{
						SetCurrentValue(SelectionEndProperty, caretIndex);
					}
				}
				else
				{
					SetCurrentValue(SelectionStartProperty, caretIndex);
					SetCurrentValue(SelectionEndProperty, caretIndex);
					_wordSelectionStart = -1;
				}
				break;
			case 2:
				if (!StringUtils.IsStartOfWord(text, caretIndex))
				{
					selectionStart = StringUtils.PreviousWord(text, caretIndex);
				}
				if (!StringUtils.IsEndOfWord(text, caretIndex))
				{
					selectionEnd = StringUtils.NextWord(text, caretIndex);
				}
				if (selectionStart != selectionEnd)
				{
					_wordSelectionStart = selectionStart;
				}
				SetCurrentValue(SelectionStartProperty, selectionStart);
				SetCurrentValue(SelectionEndProperty, selectionEnd);
				break;
			case 3:
				_wordSelectionStart = -1;
				SelectAll();
				break;
			}
		}
		e.Pointer.Capture(_presenter);
		e.Handled = true;
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		if (_presenter != null && e.Pointer.Captured == _presenter && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			Point point = e.GetPosition(_presenter);
			point = new Point(MathUtilities.Clamp(point.X, 0.0, Math.Max(_presenter.Bounds.Width - 1.0, 0.0)), MathUtilities.Clamp(point.Y, 0.0, Math.Max(_presenter.Bounds.Height - 1.0, 0.0)));
			_presenter.MoveCaretToPoint(point);
			int caretIndex = _presenter.CaretIndex;
			int selectionStart = SelectionStart;
			int selectionEnd = SelectionEnd;
			if (_wordSelectionStart >= 0)
			{
				UpdateWordSelectionRange(caretIndex, ref selectionStart, ref selectionEnd);
				SetCurrentValue(SelectionStartProperty, selectionStart);
				SetCurrentValue(SelectionEndProperty, selectionEnd);
			}
			else
			{
				SetCurrentValue(SelectionEndProperty, caretIndex);
			}
		}
	}

	private void UpdateWordSelectionRange(int caretIndex, ref int selectionStart, ref int selectionEnd)
	{
		string text = Text;
		if (!string.IsNullOrEmpty(text))
		{
			if (caretIndex > _wordSelectionStart)
			{
				int num = StringUtils.NextWord(text, caretIndex);
				selectionEnd = num;
				selectionStart = _wordSelectionStart;
			}
			else
			{
				int num2 = StringUtils.PreviousWord(text, caretIndex);
				selectionStart = num2;
				selectionEnd = StringUtils.NextWord(text, _wordSelectionStart);
			}
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		if (_presenter == null || e.Pointer.Captured != _presenter)
		{
			return;
		}
		if (e.InitialPressMouseButton == MouseButton.Right)
		{
			Point position = e.GetPosition(_presenter);
			_presenter.MoveCaretToPoint(position);
			int caretIndex = _presenter.CaretIndex;
			int num = Math.Min(SelectionStart, SelectionEnd);
			int num2 = Math.Max(SelectionStart, SelectionEnd);
			if (SelectionStart == SelectionEnd || caretIndex < num || caretIndex > num2)
			{
				SetCurrentValue(CaretIndexProperty, caretIndex);
				SetCurrentValue(SelectionEndProperty, caretIndex);
				SetCurrentValue(SelectionStartProperty, caretIndex);
			}
		}
		e.Pointer.Capture(null);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new TextBoxAutomationPeer(this);
	}

	protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
		if (property == TextProperty)
		{
			DataValidationErrors.SetError(this, error);
		}
	}

	internal static int CoerceCaretIndex(AvaloniaObject sender, int value)
	{
		string value2 = sender.GetValue(TextProperty);
		if (value2 == null)
		{
			return 0;
		}
		int length = value2.Length;
		if (value < 0)
		{
			return 0;
		}
		if (value > length)
		{
			return length;
		}
		if (value > 0 && value2[value - 1] == '\r' && value < length && value2[value] == '\n')
		{
			return value + 1;
		}
		return value;
	}

	public void Clear()
	{
		SetCurrentValue(TextProperty, string.Empty);
	}

	private void MoveHorizontal(int direction, bool wholeWord, bool isSelecting)
	{
		if (_presenter == null)
		{
			return;
		}
		string text = Text ?? string.Empty;
		int selectionStart = SelectionStart;
		int selectionEnd = SelectionEnd;
		if (!wholeWord)
		{
			if (isSelecting)
			{
				_presenter.MoveCaretToTextPosition(selectionEnd);
				_presenter.MoveCaretHorizontal((direction > 0) ? LogicalDirection.Forward : LogicalDirection.Backward);
				SetCurrentValue(SelectionEndProperty, _presenter.CaretIndex);
				return;
			}
			if (selectionStart != selectionEnd)
			{
				_presenter.MoveCaretToTextPosition((direction > 0) ? Math.Max(selectionStart, selectionEnd) : Math.Min(selectionStart, selectionEnd));
			}
			else
			{
				_presenter.MoveCaretHorizontal((direction > 0) ? LogicalDirection.Forward : LogicalDirection.Backward);
			}
			SetCurrentValue(CaretIndexProperty, _presenter.CaretIndex);
		}
		else
		{
			int num = ((direction <= 0) ? (StringUtils.PreviousWord(text, selectionEnd) - selectionEnd) : (StringUtils.NextWord(text, selectionEnd) - selectionEnd));
			SetCurrentValue(SelectionEndProperty, SelectionEnd + num);
			_presenter.MoveCaretToTextPosition(SelectionEnd);
			if (!isSelecting)
			{
				SetCurrentValue(CaretIndexProperty, SelectionEnd);
			}
			else
			{
				SetCurrentValue(SelectionStartProperty, selectionStart);
			}
		}
	}

	private void MoveHome(bool document)
	{
		if (_presenter != null)
		{
			int caretIndex = CaretIndex;
			if (document)
			{
				_presenter.MoveCaretToTextPosition(0);
				return;
			}
			IReadOnlyList<TextLine> textLines = _presenter.TextLayout.TextLines;
			int lineIndexFromCharacterIndex = _presenter.TextLayout.GetLineIndexFromCharacterIndex(caretIndex, trailingEdge: false);
			TextLine textLine = textLines[lineIndexFromCharacterIndex];
			_presenter.MoveCaretToTextPosition(textLine.FirstTextSourceIndex);
		}
	}

	private void MoveEnd(bool document)
	{
		if (_presenter != null)
		{
			string text = Text ?? string.Empty;
			int caretIndex = CaretIndex;
			if (document)
			{
				_presenter.MoveCaretToTextPosition(text.Length, trailingEdge: true);
				return;
			}
			IReadOnlyList<TextLine> textLines = _presenter.TextLayout.TextLines;
			int lineIndexFromCharacterIndex = _presenter.TextLayout.GetLineIndexFromCharacterIndex(caretIndex, trailingEdge: false);
			TextLine textLine = textLines[lineIndexFromCharacterIndex];
			int textPosition = textLine.FirstTextSourceIndex + textLine.Length - textLine.NewLineLength;
			_presenter.MoveCaretToTextPosition(textPosition, trailingEdge: true);
		}
	}

	private void MovePageRight()
	{
		_scrollViewer?.PageRight();
	}

	private void MovePageLeft()
	{
		_scrollViewer?.PageLeft();
	}

	private void MovePageUp()
	{
		_scrollViewer?.PageUp();
	}

	private void MovePageDown()
	{
		_scrollViewer?.PageDown();
	}

	public void SelectAll()
	{
		SetCurrentValue(SelectionStartProperty, 0);
		SetCurrentValue(SelectionEndProperty, Text?.Length ?? 0);
	}

	private (int start, int end) GetSelectionRange()
	{
		int selectionStart = SelectionStart;
		int selectionEnd = SelectionEnd;
		return (start: Math.Min(selectionStart, selectionEnd), end: Math.Max(selectionStart, selectionEnd));
	}

	internal bool DeleteSelection()
	{
		if (IsReadOnly)
		{
			return true;
		}
		var (num, num2) = GetSelectionRange();
		if (num != num2)
		{
			string text = Text;
			StringBuilder stringBuilder = StringBuilderCache.Acquire(text.Length);
			stringBuilder.Append(text);
			stringBuilder.Remove(num, num2 - num);
			SetCurrentValue(TextProperty, stringBuilder.ToString());
			_presenter?.MoveCaretToTextPosition(num);
			SetCurrentValue(CaretIndexProperty, num);
			ClearSelection();
			return true;
		}
		SetCurrentValue(CaretIndexProperty, SelectionStart);
		return false;
	}

	private string GetSelection()
	{
		string text = Text;
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		int selectionStart = SelectionStart;
		int selectionEnd = SelectionEnd;
		int num = Math.Min(selectionStart, selectionEnd);
		int num2 = Math.Max(selectionStart, selectionEnd);
		if (num == num2 || (Text?.Length ?? 0) < num2)
		{
			return "";
		}
		return text.Substring(num, num2 - num);
	}

	private void RaiseTextChangeEvents()
	{
		TextChangingEventArgs e = new TextChangingEventArgs(TextChangingEvent);
		RaiseEvent(e);
		Dispatcher.UIThread.Post(delegate
		{
			TextChangedEventArgs e2 = new TextChangedEventArgs(TextChangedEvent);
			RaiseEvent(e2);
		}, DispatcherPriority.Normal);
	}

	private void SetSelectionForControlBackspace()
	{
		int caretIndex = CaretIndex;
		MoveHorizontal(-1, wholeWord: true, isSelecting: false);
		SetCurrentValue(SelectionStartProperty, caretIndex);
	}

	private void SetSelectionForControlDelete()
	{
		int num = Text?.Length ?? 0;
		if (_presenter != null && num != 0)
		{
			SetCurrentValue(SelectionStartProperty, CaretIndex);
			MoveHorizontal(1, wholeWord: true, isSelecting: true);
			if (SelectionEnd < num && Text[SelectionEnd] == ' ')
			{
				SetCurrentValue(SelectionEndProperty, SelectionEnd + 1);
			}
		}
	}

	private void UpdatePseudoclasses()
	{
		base.PseudoClasses.Set(":empty", string.IsNullOrEmpty(Text));
	}

	private void SnapshotUndoRedo(bool ignoreChangeCount = true)
	{
		if (IsUndoEnabled && (ignoreChangeCount || !_hasDoneSnapshotOnce || (!ignoreChangeCount && _selectedTextChangesMadeSinceLastUndoSnapshot >= 7)))
		{
			_undoRedoHelper.Snapshot();
			_selectedTextChangesMadeSinceLastUndoSnapshot = 0;
			_hasDoneSnapshotOnce = true;
		}
	}

	public void Undo()
	{
		if (IsUndoEnabled && CanUndo)
		{
			try
			{
				SnapshotUndoRedo();
				_isUndoingRedoing = true;
				_undoRedoHelper.Undo();
			}
			finally
			{
				_isUndoingRedoing = false;
			}
		}
	}

	public void Redo()
	{
		if (IsUndoEnabled && CanRedo)
		{
			try
			{
				_isUndoingRedoing = true;
				_undoRedoHelper.Redo();
			}
			finally
			{
				_isUndoingRedoing = false;
			}
		}
	}

	void UndoRedoHelper<UndoRedoState>.IUndoRedoHost.OnUndoStackChanged()
	{
		CanUndo = _undoRedoHelper.CanUndo;
	}

	void UndoRedoHelper<UndoRedoState>.IUndoRedoHost.OnRedoStackChanged()
	{
		CanRedo = _undoRedoHelper.CanRedo;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (_scrollViewer != null)
		{
			double value = double.PositiveInfinity;
			if (MaxLines > 0 && double.IsNaN(base.Height))
			{
				double fontSize = base.FontSize;
				TextParagraphProperties paragraphProperties = TextLayout.CreateTextParagraphProperties(new Typeface(base.FontFamily, base.FontStyle, base.FontWeight, base.FontStretch), fontSize, null, TextAlignment.Left, TextWrapping.NoWrap, null, FlowDirection.LeftToRight, LineHeight, 0.0);
				value = Math.Ceiling(new TextLayout(new MaxLinesTextSource(MaxLines), paragraphProperties).Height);
			}
			_scrollViewer.SetCurrentValue(Layoutable.MaxHeightProperty, value);
		}
		return base.MeasureOverride(availableSize);
	}
}
