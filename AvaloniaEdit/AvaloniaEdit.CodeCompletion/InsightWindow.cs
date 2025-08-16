using System;
using AvaloniaEdit.Editing;

namespace AvaloniaEdit.CodeCompletion;

public class InsightWindow : CompletionWindowBase
{
	public bool CloseAutomatically { get; set; }

	protected override bool CloseOnFocusLost => CloseAutomatically;

	public InsightWindow(TextArea textArea)
		: base(textArea)
	{
		CloseAutomatically = true;
		AttachEvents();
		Initialize();
	}

	private void Initialize()
	{
	}

	private void AttachEvents()
	{
		base.TextArea.Caret.PositionChanged += CaretPositionChanged;
	}

	protected override void DetachEvents()
	{
		base.TextArea.Caret.PositionChanged -= CaretPositionChanged;
		base.DetachEvents();
	}

	private void CaretPositionChanged(object sender, EventArgs e)
	{
		if (CloseAutomatically)
		{
			int offset = base.TextArea.Caret.Offset;
			if (offset < base.StartOffset || offset > base.EndOffset)
			{
				Hide();
			}
		}
	}
}
