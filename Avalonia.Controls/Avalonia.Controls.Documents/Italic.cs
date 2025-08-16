using Avalonia.Media;

namespace Avalonia.Controls.Documents;

public sealed class Italic : Span
{
	static Italic()
	{
		TextElement.FontStyleProperty.OverrideDefaultValue<Italic>(FontStyle.Italic);
	}
}
