using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Utils;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class SelectableTextBlock : TextBlock, IInlineHost, ILogical
{
	public static readonly StyledProperty<int> SelectionStartProperty;

	public static readonly StyledProperty<int> SelectionEndProperty;

	public static readonly DirectProperty<SelectableTextBlock, string> SelectedTextProperty;

	public static readonly StyledProperty<IBrush?> SelectionBrushProperty;

	public static readonly DirectProperty<SelectableTextBlock, bool> CanCopyProperty;

	public static readonly RoutedEvent<RoutedEventArgs> CopyingToClipboardEvent;

	private bool _canCopy;

	private int _wordSelectionStart = -1;

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

	public string SelectedText => GetSelection();

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

	static SelectableTextBlock()
	{
		SelectionStartProperty = TextBox.SelectionStartProperty.AddOwner<SelectableTextBlock>();
		SelectionEndProperty = TextBox.SelectionEndProperty.AddOwner<SelectableTextBlock>();
		SelectedTextProperty = AvaloniaProperty.RegisterDirect("SelectedText", (SelectableTextBlock o) => o.SelectedText);
		SelectionBrushProperty = TextBox.SelectionBrushProperty.AddOwner<SelectableTextBlock>();
		CanCopyProperty = TextBox.CanCopyProperty.AddOwner((SelectableTextBlock o) => o.CanCopy, null, unsetValue: false);
		CopyingToClipboardEvent = RoutedEvent.Register<SelectableTextBlock, RoutedEventArgs>("CopyingToClipboard", RoutingStrategies.Bubble);
		InputElement.FocusableProperty.OverrideDefaultValue(typeof(SelectableTextBlock), defaultValue: true);
		Visual.AffectsRender<SelectableTextBlock>(new AvaloniaProperty[3] { SelectionStartProperty, SelectionEndProperty, SelectionBrushProperty });
	}

	public async void Copy()
	{
		if (!_canCopy)
		{
			return;
		}
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

	public void SelectAll()
	{
		string text = base.Text;
		SetCurrentValue(SelectionStartProperty, 0);
		SetCurrentValue(SelectionEndProperty, text?.Length ?? 0);
	}

	public void ClearSelection()
	{
		SetCurrentValue(SelectionEndProperty, SelectionStart);
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		UpdateCommandStates();
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		if ((base.ContextFlyout == null || !base.ContextFlyout.IsOpen) && (base.ContextMenu == null || !base.ContextMenu.IsOpen))
		{
			ClearSelection();
		}
		UpdateCommandStates();
	}

	protected override void RenderTextLayout(DrawingContext context, Point origin)
	{
		int selectionStart = SelectionStart;
		int selectionEnd = SelectionEnd;
		IBrush selectionBrush = SelectionBrush;
		if (selectionStart != selectionEnd && selectionBrush != null)
		{
			int num = Math.Min(selectionStart, selectionEnd);
			int length = Math.Max(selectionStart, selectionEnd) - num;
			IEnumerable<Rect> enumerable = base.TextLayout.HitTestTextRange(num, length);
			using (context.PushTransform(Matrix.CreateTranslation(origin)))
			{
				foreach (Rect item in enumerable)
				{
					context.FillRectangle(selectionBrush, PixelRect.FromRect(item, 1.0).ToRect(1.0));
				}
			}
		}
		base.RenderTextLayout(context, origin);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		bool handled = false;
		_ = e.KeyModifiers;
		PlatformHotkeyConfiguration hotkeyConfiguration = Application.Current.PlatformSettings.HotkeyConfiguration;
		if (Match(hotkeyConfiguration.Copy))
		{
			Copy();
			handled = true;
		}
		else if (Match(hotkeyConfiguration.SelectAll))
		{
			SelectAll();
			handled = true;
		}
		e.Handled = handled;
		bool Match(List<KeyGesture> gestures)
		{
			return gestures.Any((KeyGesture g) => g.Matches(e));
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == SelectionStartProperty || change.Property == SelectionEndProperty)
		{
			RaisePropertyChanged(SelectedTextProperty, "", "");
			UpdateCommandStates();
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		string text = ((!base.HasComplexContent) ? base.Text : base.Inlines?.Text);
		PointerPoint currentPoint = e.GetCurrentPoint(this);
		if (text != null && currentPoint.Properties.IsLeftButtonPressed)
		{
			Thickness padding = base.Padding;
			Point point = e.GetPosition(this) - new Point(padding.Left, padding.Top);
			bool flag = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
			int selectionStart = SelectionStart;
			int textPosition = base.TextLayout.HitTestPoint(in point).TextPosition;
			switch (e.ClickCount)
			{
			case 1:
				if (flag)
				{
					if (_wordSelectionStart >= 0)
					{
						int num = StringUtils.PreviousWord(text, textPosition);
						if (textPosition > _wordSelectionStart)
						{
							SetCurrentValue(SelectionEndProperty, StringUtils.NextWord(text, textPosition));
						}
						if (textPosition < _wordSelectionStart || num == _wordSelectionStart)
						{
							SetCurrentValue(SelectionStartProperty, num);
						}
					}
					else
					{
						SetCurrentValue(SelectionStartProperty, Math.Min(selectionStart, textPosition));
						SetCurrentValue(SelectionEndProperty, Math.Max(selectionStart, textPosition));
					}
				}
				else if (_wordSelectionStart == -1 || textPosition < SelectionStart || textPosition > SelectionEnd)
				{
					SetCurrentValue(SelectionStartProperty, textPosition);
					SetCurrentValue(SelectionEndProperty, textPosition);
					_wordSelectionStart = -1;
				}
				break;
			case 2:
				if (!StringUtils.IsStartOfWord(text, textPosition))
				{
					SetCurrentValue(SelectionStartProperty, StringUtils.PreviousWord(text, textPosition));
				}
				_wordSelectionStart = SelectionStart;
				if (!StringUtils.IsEndOfWord(text, textPosition))
				{
					SetCurrentValue(SelectionEndProperty, StringUtils.NextWord(text, textPosition));
				}
				break;
			case 3:
				_wordSelectionStart = -1;
				SelectAll();
				break;
			}
		}
		e.Pointer.Capture(this);
		e.Handled = true;
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		base.OnPointerMoved(e);
		if (e.Pointer.Captured != this || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			return;
		}
		string text = ((!base.HasComplexContent) ? base.Text : base.Inlines?.Text);
		Thickness padding = base.Padding;
		Point point = e.GetPosition(this) - new Point(padding.Left, padding.Top);
		point = new Point(MathUtilities.Clamp(point.X, 0.0, Math.Max(base.TextLayout.WidthIncludingTrailingWhitespace, 0.0)), MathUtilities.Clamp(point.Y, 0.0, Math.Max(base.TextLayout.Height, 0.0)));
		int textPosition = base.TextLayout.HitTestPoint(in point).TextPosition;
		if (text != null && _wordSelectionStart >= 0)
		{
			int num = textPosition - _wordSelectionStart;
			if (num <= 0)
			{
				SetCurrentValue(SelectionStartProperty, StringUtils.PreviousWord(text, textPosition));
			}
			if (num >= 0)
			{
				if (SelectionStart != _wordSelectionStart)
				{
					SetCurrentValue(SelectionStartProperty, _wordSelectionStart);
				}
				SetCurrentValue(SelectionEndProperty, StringUtils.NextWord(text, textPosition));
			}
		}
		else
		{
			SetCurrentValue(SelectionEndProperty, textPosition);
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (e.Pointer.Captured != this)
		{
			return;
		}
		if (e.InitialPressMouseButton == MouseButton.Right)
		{
			Thickness padding = base.Padding;
			Point point = e.GetPosition(this) - new Point(padding.Left, padding.Top);
			int textPosition = base.TextLayout.HitTestPoint(in point).TextPosition;
			int num = Math.Min(SelectionStart, SelectionEnd);
			int num2 = Math.Max(SelectionStart, SelectionEnd);
			if (SelectionStart == SelectionEnd || textPosition < num || textPosition > num2)
			{
				SetCurrentValue(SelectionStartProperty, textPosition);
				SetCurrentValue(SelectionEndProperty, textPosition);
			}
		}
		e.Pointer.Capture(null);
	}

	private void UpdateCommandStates()
	{
		string selection = GetSelection();
		CanCopy = !string.IsNullOrEmpty(selection);
	}

	private string GetSelection()
	{
		string text = ((!base.HasComplexContent) ? base.Text : base.Inlines?.Text);
		int num = text?.Length ?? 0;
		if (num == 0)
		{
			return "";
		}
		int selectionStart = SelectionStart;
		int selectionEnd = SelectionEnd;
		int num2 = Math.Min(selectionStart, selectionEnd);
		int num3 = Math.Max(selectionStart, selectionEnd);
		if (num2 == num3 || num < num3)
		{
			return "";
		}
		int length = Math.Max(0, num3 - num2);
		return text.Substring(num2, length);
	}
}
