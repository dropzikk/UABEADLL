using System;
using Avalonia.Media.Immutable;

namespace Avalonia.Media;

public static class BrushExtensions
{
	public static IImmutableBrush ToImmutable(this IBrush brush)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		return (brush as IMutableBrush)?.ToImmutable() ?? ((IImmutableBrush)brush);
	}

	public static ImmutableDashStyle ToImmutable(this IDashStyle style)
	{
		if (style == null)
		{
			throw new ArgumentNullException("style");
		}
		return (style as ImmutableDashStyle) ?? ((DashStyle)style).ToImmutable();
	}

	public static ImmutablePen ToImmutable(this IPen pen)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		return (pen as ImmutablePen) ?? ((Pen)pen).ToImmutable();
	}
}
