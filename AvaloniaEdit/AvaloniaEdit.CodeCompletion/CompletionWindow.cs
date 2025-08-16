using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;

namespace AvaloniaEdit.CodeCompletion;

public class CompletionWindow : CompletionWindowBase
{
	private PopupWithCustomPosition _toolTip;

	private ContentControl _toolTipContent;

	public CompletionList CompletionList { get; }

	public bool CloseAutomatically { get; set; }

	protected override bool CloseOnFocusLost => CloseAutomatically;

	public bool CloseWhenCaretAtBeginning { get; set; }

	public CompletionWindow(TextArea textArea)
		: base(textArea)
	{
		CompletionList = new CompletionList();
		CloseAutomatically = true;
		base.MaxHeight = 225.0;
		base.Width = 175.0;
		base.Child = CompletionList;
		base.MinHeight = 15.0;
		base.MinWidth = 30.0;
		_toolTipContent = new ContentControl();
		_toolTipContent.Classes.Add("ToolTip");
		_toolTip = new PopupWithCustomPosition
		{
			IsLightDismissEnabled = true,
			PlacementTarget = this,
			Placement = PlacementMode.RightEdgeAlignedTop,
			Child = _toolTipContent
		};
		base.LogicalChildren.Add(_toolTip);
		AttachEvents();
	}

	protected override void OnClosed()
	{
		base.OnClosed();
		if (_toolTip != null)
		{
			_toolTip.IsOpen = false;
			_toolTip = null;
			_toolTipContent = null;
		}
	}

	private void CompletionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_toolTipContent == null)
		{
			return;
		}
		ICompletionData selectedItem = CompletionList.SelectedItem;
		object obj = selectedItem?.Description;
		if (obj != null)
		{
			if (obj is string text)
			{
				_toolTipContent.Content = new TextBlock
				{
					Text = text,
					TextWrapping = TextWrapping.Wrap
				};
			}
			else
			{
				_toolTipContent.Content = obj;
			}
			_toolTip.IsOpen = false;
			PopupRoot popupRoot = base.Host as PopupRoot;
			if (CompletionList.CurrentList != null)
			{
				double y = 0.0;
				Control control = CompletionList.ListBox.ContainerFromItem(selectedItem);
				if (popupRoot != null && control != null)
				{
					Point? point = control.TranslatePoint(new Point(0.0, 0.0), popupRoot);
					if (point.HasValue)
					{
						y = point.Value.Y;
					}
				}
				_toolTip.Offset = new Point(2.0, y);
			}
			_toolTip.PlacementTarget = popupRoot;
			_toolTip.IsOpen = true;
		}
		else
		{
			_toolTip.IsOpen = false;
		}
	}

	private void CompletionList_InsertionRequested(object sender, EventArgs e)
	{
		Hide();
		CompletionList.SelectedItem?.Complete(base.TextArea, new AnchorSegment(base.TextArea.Document, base.StartOffset, base.EndOffset - base.StartOffset), e);
	}

	private void AttachEvents()
	{
		CompletionList.InsertionRequested += CompletionList_InsertionRequested;
		CompletionList.SelectionChanged += CompletionList_SelectionChanged;
		base.TextArea.Caret.PositionChanged += CaretPositionChanged;
		base.TextArea.PointerWheelChanged += TextArea_MouseWheel;
		base.TextArea.TextInput += TextArea_PreviewTextInput;
	}

	protected override void DetachEvents()
	{
		CompletionList.InsertionRequested -= CompletionList_InsertionRequested;
		CompletionList.SelectionChanged -= CompletionList_SelectionChanged;
		base.TextArea.Caret.PositionChanged -= CaretPositionChanged;
		base.TextArea.PointerWheelChanged -= TextArea_MouseWheel;
		base.TextArea.TextInput -= TextArea_PreviewTextInput;
		base.DetachEvents();
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled)
		{
			CompletionList.HandleKey(e);
		}
	}

	private void TextArea_PreviewTextInput(object sender, TextInputEventArgs e)
	{
		e.Handled = CompletionWindowBase.RaiseEventPair(this, null, InputElement.TextInputEvent, new TextInputEventArgs
		{
			Text = e.Text
		});
	}

	private void TextArea_MouseWheel(object sender, PointerWheelEventArgs e)
	{
		e.Handled = CompletionWindowBase.RaiseEventPair(GetScrollEventTarget(), null, InputElement.PointerWheelChangedEvent, e);
	}

	private Control GetScrollEventTarget()
	{
		if (CompletionList == null)
		{
			return this;
		}
		return (Control)(CompletionList.ScrollViewer ?? ((object)CompletionList.ListBox) ?? ((object)CompletionList));
	}

	private void CaretPositionChanged(object sender, EventArgs e)
	{
		int offset = base.TextArea.Caret.Offset;
		if (offset == base.StartOffset)
		{
			if (CloseAutomatically && CloseWhenCaretAtBeginning)
			{
				Hide();
				return;
			}
			CompletionList.SelectItem(string.Empty);
			if (CompletionList.ListBox.ItemCount == 0)
			{
				base.IsVisible = false;
			}
			else
			{
				base.IsVisible = true;
			}
			return;
		}
		if (offset < base.StartOffset || offset > base.EndOffset)
		{
			if (CloseAutomatically)
			{
				Hide();
			}
			return;
		}
		TextDocument document = base.TextArea.Document;
		if (document != null)
		{
			CompletionList.SelectItem(document.GetText(base.StartOffset, offset - base.StartOffset));
			if (CompletionList.ListBox.ItemCount == 0)
			{
				base.IsVisible = false;
			}
			else
			{
				base.IsVisible = true;
			}
		}
	}
}
