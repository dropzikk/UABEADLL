using System;

namespace Avalonia.Controls.Primitives.PopupPositioning;

internal static class PopupPositioningEdgeHelper
{
	public static void ValidateEdge(this PopupAnchor edge)
	{
		if (edge.HasAllFlags(PopupAnchor.HorizontalMask) || edge.HasAllFlags(PopupAnchor.VerticalMask))
		{
			throw new ArgumentException("Opposite edges specified");
		}
	}

	public static void ValidateGravity(this PopupGravity gravity)
	{
		((PopupAnchor)gravity).ValidateEdge();
	}

	public static PopupAnchor Flip(this PopupAnchor edge)
	{
		if (edge.HasAnyFlag(PopupAnchor.HorizontalMask))
		{
			edge ^= PopupAnchor.HorizontalMask;
		}
		if (edge.HasAnyFlag(PopupAnchor.VerticalMask))
		{
			edge ^= PopupAnchor.VerticalMask;
		}
		return edge;
	}

	public static PopupAnchor FlipX(this PopupAnchor edge)
	{
		if (edge.HasAnyFlag(PopupAnchor.HorizontalMask))
		{
			edge ^= PopupAnchor.HorizontalMask;
		}
		return edge;
	}

	public static PopupAnchor FlipY(this PopupAnchor edge)
	{
		if (edge.HasAnyFlag(PopupAnchor.VerticalMask))
		{
			edge ^= PopupAnchor.VerticalMask;
		}
		return edge;
	}

	public static PopupGravity FlipX(this PopupGravity gravity)
	{
		return (PopupGravity)((PopupAnchor)gravity).FlipX();
	}

	public static PopupGravity FlipY(this PopupGravity gravity)
	{
		return (PopupGravity)((PopupAnchor)gravity).FlipY();
	}
}
