using Avalonia.Media;

namespace Avalonia.Controls.Documents;

public sealed class Underline : Span
{
	static Underline()
	{
		Inline.TextDecorationsProperty.OverrideDefaultValue<Underline>(Avalonia.Media.TextDecorations.Underline);
	}
}
