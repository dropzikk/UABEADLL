using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using AvaloniaEdit.Editing;

namespace AvaloniaEdit.Snippets;

internal sealed class SnippetInputHandler : TextAreaStackedInputHandler
{
	private readonly InsertionContext _context;

	public SnippetInputHandler(InsertionContext context)
		: base(context.TextArea)
	{
		_context = context;
	}

	public override void Attach()
	{
		base.Attach();
		SelectElement(FindNextEditableElement(-1, backwards: false));
	}

	public override void Detach()
	{
		base.Detach();
		_context.Deactivate(new SnippetEventArgs(DeactivateReason.InputHandlerDetached));
	}

	public override void OnPreviewKeyDown(KeyEventArgs e)
	{
		base.OnPreviewKeyDown(e);
		if (e.Key == Key.Escape)
		{
			_context.Deactivate(new SnippetEventArgs(DeactivateReason.EscapePressed));
			e.Handled = true;
		}
		else if (e.Key == Key.Return)
		{
			_context.Deactivate(new SnippetEventArgs(DeactivateReason.ReturnPressed));
			e.Handled = true;
		}
		else if (e.Key == Key.Tab)
		{
			bool backwards = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
			SelectElement(FindNextEditableElement(base.TextArea.Caret.Offset, backwards));
			e.Handled = true;
		}
	}

	private void SelectElement(IActiveElement element)
	{
		if (element != null)
		{
			base.TextArea.Selection = Selection.Create(base.TextArea, element.Segment);
			base.TextArea.Caret.Offset = element.Segment.EndOffset;
		}
	}

	private IActiveElement FindNextEditableElement(int offset, bool backwards)
	{
		IEnumerable<IActiveElement> enumerable = _context.ActiveElements.Where((IActiveElement e) => e.IsEditable && e.Segment != null);
		if (backwards)
		{
			enumerable = enumerable.Reverse();
			foreach (IActiveElement item in enumerable)
			{
				if (offset > item.Segment.EndOffset)
				{
					return item;
				}
			}
		}
		else
		{
			foreach (IActiveElement item2 in enumerable)
			{
				if (offset < item2.Segment.Offset)
				{
					return item2;
				}
			}
		}
		return enumerable.FirstOrDefault();
	}
}
