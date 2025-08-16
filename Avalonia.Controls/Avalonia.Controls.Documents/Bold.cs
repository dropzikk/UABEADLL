using Avalonia.Media;

namespace Avalonia.Controls.Documents;

public sealed class Bold : Span
{
	static Bold()
	{
		TextElement.FontWeightProperty.OverrideDefaultValue<Bold>(FontWeight.Bold);
	}
}
