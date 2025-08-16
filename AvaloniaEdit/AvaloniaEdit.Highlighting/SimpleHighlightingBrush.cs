using Avalonia.Media;
using Avalonia.Media.Immutable;
using AvaloniaEdit.Rendering;

namespace AvaloniaEdit.Highlighting;

public sealed class SimpleHighlightingBrush : HighlightingBrush
{
	private readonly ISolidColorBrush _brush;

	internal SimpleHighlightingBrush(ISolidColorBrush brush)
	{
		_brush = brush;
	}

	public SimpleHighlightingBrush(Color color)
		: this(new ImmutableSolidColorBrush(color))
	{
	}

	public override IBrush GetBrush(ITextRunConstructionContext context)
	{
		return _brush;
	}

	public override string ToString()
	{
		return _brush.ToString();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SimpleHighlightingBrush simpleHighlightingBrush))
		{
			return false;
		}
		return _brush.Color.Equals(simpleHighlightingBrush._brush.Color);
	}

	public override int GetHashCode()
	{
		return _brush.Color.GetHashCode();
	}
}
