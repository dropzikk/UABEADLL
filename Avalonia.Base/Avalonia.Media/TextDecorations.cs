namespace Avalonia.Media;

public static class TextDecorations
{
	public static TextDecorationCollection Underline { get; }

	public static TextDecorationCollection Strikethrough { get; }

	public static TextDecorationCollection Overline { get; }

	public static TextDecorationCollection Baseline { get; }

	static TextDecorations()
	{
		Underline = new TextDecorationCollection
		{
			new TextDecoration
			{
				Location = TextDecorationLocation.Underline
			}
		};
		Strikethrough = new TextDecorationCollection
		{
			new TextDecoration
			{
				Location = TextDecorationLocation.Strikethrough
			}
		};
		Overline = new TextDecorationCollection
		{
			new TextDecoration
			{
				Location = TextDecorationLocation.Overline
			}
		};
		Baseline = new TextDecorationCollection
		{
			new TextDecoration
			{
				Location = TextDecorationLocation.Baseline
			}
		};
	}
}
