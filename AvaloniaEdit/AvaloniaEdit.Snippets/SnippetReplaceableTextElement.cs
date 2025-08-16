namespace AvaloniaEdit.Snippets;

public class SnippetReplaceableTextElement : SnippetTextElement
{
	public override void Insert(InsertionContext context)
	{
		int insertionPosition = context.InsertionPosition;
		base.Insert(context);
		int insertionPosition2 = context.InsertionPosition;
		context.RegisterActiveElement(this, new ReplaceableActiveElement(context, insertionPosition, insertionPosition2));
	}
}
