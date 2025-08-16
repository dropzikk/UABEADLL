using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;

namespace AvaloniaEdit.CodeCompletion;

public class CompletionWindowBase : Popup
{
	private sealed class InputHandler : TextAreaStackedInputHandler
	{
		internal readonly CompletionWindowBase Window;

		public InputHandler(CompletionWindowBase window)
			: base(window.TextArea)
		{
			Window = window;
		}

		public override void Detach()
		{
			base.Detach();
			Window.Hide();
		}

		public override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key != Key.DeadCharProcessed)
			{
				e.Handled = RaiseEventPair(Window, null, InputElement.KeyDownEvent, new KeyEventArgs
				{
					Key = e.Key
				});
			}
		}

		public override void OnPreviewKeyUp(KeyEventArgs e)
		{
			if (e.Key != Key.DeadCharProcessed)
			{
				e.Handled = RaiseEventPair(Window, null, InputElement.KeyUpEvent, new KeyEventArgs
				{
					Key = e.Key
				});
			}
		}
	}

	private readonly Window _parentWindow;

	private TextDocument _document;

	private InputHandler _myInputHandler;

	private Point _visualLocation;

	private Point _visualLocationTop;

	protected override Type StyleKeyOverride => typeof(PopupRoot);

	public TextArea TextArea { get; }

	public int StartOffset { get; set; }

	public int EndOffset { get; set; }

	protected bool IsUp { get; private set; }

	protected virtual bool CloseOnFocusLost => true;

	private bool IsTextAreaFocused
	{
		get
		{
			if (_parentWindow != null && !_parentWindow.IsActive)
			{
				return false;
			}
			return TextArea.IsFocused;
		}
	}

	public bool ExpectInsertionBeforeStart { get; set; }

	static CompletionWindowBase()
	{
	}

	public CompletionWindowBase(TextArea textArea)
	{
		TextArea = textArea ?? throw new ArgumentNullException("textArea");
		_parentWindow = textArea.GetVisualRoot() as Window;
		AddHandler(InputElement.PointerReleasedEvent, OnMouseUp, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true);
		StartOffset = (EndOffset = TextArea.Caret.Offset);
		base.PlacementTarget = TextArea.TextView;
		base.Placement = PlacementMode.AnchorAndGravity;
		base.PlacementAnchor = PopupAnchor.TopLeft;
		base.PlacementGravity = PopupGravity.BottomRight;
		base.Closed += delegate
		{
			DetachEvents();
		};
		AttachEvents();
		Initialize();
	}

	protected virtual void OnClosed()
	{
		DetachEvents();
	}

	private void Initialize()
	{
		if (_document != null && StartOffset != TextArea.Caret.Offset)
		{
			SetPosition(new TextViewPosition(_document.GetLocation(StartOffset)));
		}
		else
		{
			SetPosition(TextArea.Caret.Position);
		}
	}

	public void Show()
	{
		UpdatePosition();
		Open();
		base.Height = double.NaN;
		base.MinHeight = 0.0;
	}

	public void Hide()
	{
		Close();
		OnClosed();
	}

	private void AttachEvents()
	{
		((ISetLogicalParent)this).SetParent(TextArea.GetVisualRoot() as ILogical);
		_document = TextArea.Document;
		if (_document != null)
		{
			_document.Changing += TextArea_Document_Changing;
		}
		TextArea.LostFocus += TextAreaLostFocus;
		TextArea.TextView.ScrollOffsetChanged += TextViewScrollOffsetChanged;
		TextArea.DocumentChanged += TextAreaDocumentChanged;
		if (_parentWindow != null)
		{
			_parentWindow.PositionChanged += ParentWindow_LocationChanged;
			_parentWindow.Deactivated += ParentWindow_Deactivated;
		}
		foreach (InputHandler item in TextArea.StackedInputHandlers.OfType<InputHandler>())
		{
			if (item.Window.GetType() == GetType())
			{
				TextArea.PopStackedInputHandler(item);
			}
		}
		_myInputHandler = new InputHandler(this);
		TextArea.PushStackedInputHandler(_myInputHandler);
	}

	protected virtual void DetachEvents()
	{
		((ISetLogicalParent)this).SetParent((ILogical?)null);
		if (_document != null)
		{
			_document.Changing -= TextArea_Document_Changing;
		}
		TextArea.LostFocus -= TextAreaLostFocus;
		TextArea.TextView.ScrollOffsetChanged -= TextViewScrollOffsetChanged;
		TextArea.DocumentChanged -= TextAreaDocumentChanged;
		if (_parentWindow != null)
		{
			_parentWindow.PositionChanged -= ParentWindow_LocationChanged;
			_parentWindow.Deactivated -= ParentWindow_Deactivated;
		}
		TextArea.PopStackedInputHandler(_myInputHandler);
	}

	private void TextViewScrollOffsetChanged(object sender, EventArgs e)
	{
		ILogicalScrollable textArea = TextArea;
		Rect rect = new Rect(textArea.Offset.X, textArea.Offset.Y, textArea.Viewport.Width, textArea.Viewport.Height);
		if (rect.Contains(_visualLocation) || rect.Contains(_visualLocationTop))
		{
			UpdatePosition();
		}
		else
		{
			Hide();
		}
	}

	private void TextAreaDocumentChanged(object sender, EventArgs e)
	{
		Hide();
	}

	private void TextAreaLostFocus(object sender, RoutedEventArgs e)
	{
		Dispatcher.UIThread.Post(CloseIfFocusLost, DispatcherPriority.Background);
	}

	private void ParentWindow_Deactivated(object sender, EventArgs e)
	{
		Hide();
	}

	private void ParentWindow_LocationChanged(object sender, EventArgs e)
	{
		UpdatePosition();
	}

	private void OnDeactivated(object sender, EventArgs e)
	{
		Dispatcher.UIThread.Post(CloseIfFocusLost, DispatcherPriority.Background);
	}

	protected static bool RaiseEventPair(Control target, RoutedEvent previewEvent, RoutedEvent @event, RoutedEventArgs args)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (args == null)
		{
			throw new ArgumentNullException("args");
		}
		if (previewEvent != null)
		{
			args.RoutedEvent = previewEvent;
			target.RaiseEvent(args);
		}
		args.RoutedEvent = @event ?? throw new ArgumentNullException("event");
		target.RaiseEvent(args);
		return args.Handled;
	}

	private void OnMouseUp(object sender, PointerReleasedEventArgs e)
	{
		ActivateParentWindow();
	}

	protected virtual void ActivateParentWindow()
	{
		_parentWindow?.Activate();
	}

	private void CloseIfFocusLost()
	{
		if (CloseOnFocusLost && !base.IsFocused && !IsTextAreaFocused)
		{
			Hide();
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled && e.Key == Key.Escape)
		{
			e.Handled = true;
			Hide();
		}
	}

	protected void SetPosition(TextViewPosition position)
	{
		TextView textView = TextArea.TextView;
		_visualLocation = textView.GetVisualPosition(position, VisualYPosition.LineBottom);
		_visualLocationTop = textView.GetVisualPosition(position, VisualYPosition.LineTop);
		UpdatePosition();
	}

	protected void UpdatePosition()
	{
		TextView textView = TextArea.TextView;
		Point point = _visualLocation - textView.ScrollOffset;
		base.HorizontalOffset = point.X;
		base.VerticalOffset = point.Y;
	}

	private void TextArea_Document_Changing(object sender, DocumentChangeEventArgs e)
	{
		if (e.Offset + e.RemovalLength == StartOffset && e.RemovalLength > 0)
		{
			Hide();
		}
		if (e.Offset == StartOffset && e.RemovalLength == 0 && ExpectInsertionBeforeStart)
		{
			StartOffset = e.GetNewOffset(StartOffset, AnchorMovementType.AfterInsertion);
			ExpectInsertionBeforeStart = false;
		}
		else
		{
			StartOffset = e.GetNewOffset(StartOffset, AnchorMovementType.BeforeInsertion);
		}
		EndOffset = e.GetNewOffset(EndOffset, AnchorMovementType.AfterInsertion);
	}
}
