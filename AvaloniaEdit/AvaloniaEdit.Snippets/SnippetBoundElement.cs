using AvaloniaEdit.Document;

namespace AvaloniaEdit.Snippets;

public class SnippetBoundElement : SnippetElement
{
	public SnippetReplaceableTextElement TargetElement { get; set; }

	public virtual string ConvertText(string input)
	{
		return input;
	}

	public override void Insert(InsertionContext context)
	{
		if (TargetElement != null)
		{
			TextAnchor textAnchor = context.Document.CreateAnchor(context.InsertionPosition);
			textAnchor.MovementType = AnchorMovementType.BeforeInsertion;
			textAnchor.SurviveDeletion = true;
			string text = TargetElement.Text;
			if (text != null)
			{
				context.InsertText(ConvertText(text));
			}
			TextAnchor textAnchor2 = context.Document.CreateAnchor(context.InsertionPosition);
			textAnchor2.MovementType = AnchorMovementType.BeforeInsertion;
			textAnchor2.SurviveDeletion = true;
			AnchorSegment segment = new AnchorSegment(textAnchor, textAnchor2);
			context.RegisterActiveElement(this, new BoundActiveElement(context, TargetElement, this, segment));
		}
	}
}
