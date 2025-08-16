using AvaloniaEdit.Document;

namespace AvaloniaEdit.Snippets;

public class SnippetCaretElement : SnippetElement
{
	private readonly bool _setCaretOnlyIfTextIsSelected;

	public SnippetCaretElement()
	{
	}

	public SnippetCaretElement(bool setCaretOnlyIfTextIsSelected)
	{
		_setCaretOnlyIfTextIsSelected = setCaretOnlyIfTextIsSelected;
	}

	public override void Insert(InsertionContext context)
	{
		if (!_setCaretOnlyIfTextIsSelected || !string.IsNullOrEmpty(context.SelectedText))
		{
			SetCaret(context);
		}
	}

	internal static void SetCaret(InsertionContext context)
	{
		TextAnchor pos = context.Document.CreateAnchor(context.InsertionPosition);
		pos.MovementType = AnchorMovementType.BeforeInsertion;
		pos.SurviveDeletion = true;
		context.Deactivated += delegate(object sender, SnippetEventArgs e)
		{
			if (e.Reason == DeactivateReason.ReturnPressed || e.Reason == DeactivateReason.NoActiveElements)
			{
				context.TextArea.Caret.Offset = pos.Offset;
			}
		};
	}
}
