namespace AvaloniaEdit.Snippets;

public class SnippetTextElement : SnippetElement
{
	public string Text { get; set; }

	public override void Insert(InsertionContext context)
	{
		if (Text != null)
		{
			context.InsertText(Text);
		}
	}
}
