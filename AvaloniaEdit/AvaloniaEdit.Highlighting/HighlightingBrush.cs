using Avalonia.Media;
using AvaloniaEdit.Rendering;

namespace AvaloniaEdit.Highlighting;

public abstract class HighlightingBrush
{
	public abstract IBrush GetBrush(ITextRunConstructionContext context);

	public virtual Color? GetColor(ITextRunConstructionContext context)
	{
		if (GetBrush(context) is ISolidColorBrush solidColorBrush)
		{
			return solidColorBrush.Color;
		}
		return null;
	}
}
