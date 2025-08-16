using System;
using System.Collections.Generic;

namespace Avalonia.Media.Immutable;

public class ImmutablePen : IPen, IEquatable<IPen>
{
	public IBrush? Brush { get; }

	public double Thickness { get; }

	public IDashStyle? DashStyle { get; }

	public PenLineCap LineCap { get; }

	public PenLineJoin LineJoin { get; }

	public double MiterLimit { get; }

	public ImmutablePen(uint color, double thickness = 1.0, ImmutableDashStyle? dashStyle = null, PenLineCap lineCap = PenLineCap.Flat, PenLineJoin lineJoin = PenLineJoin.Miter, double miterLimit = 10.0)
		: this(new ImmutableSolidColorBrush(color), thickness, dashStyle, lineCap, lineJoin, miterLimit)
	{
	}

	public ImmutablePen(IImmutableBrush? brush, double thickness = 1.0, ImmutableDashStyle? dashStyle = null, PenLineCap lineCap = PenLineCap.Flat, PenLineJoin lineJoin = PenLineJoin.Miter, double miterLimit = 10.0)
	{
		Brush = brush;
		Thickness = thickness;
		LineCap = lineCap;
		LineJoin = lineJoin;
		MiterLimit = miterLimit;
		DashStyle = dashStyle;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IPen);
	}

	public bool Equals(IPen? other)
	{
		if (this == other)
		{
			return true;
		}
		if (other == null)
		{
			return false;
		}
		if (EqualityComparer<IBrush>.Default.Equals(Brush, other.Brush) && Thickness == other.Thickness && EqualityComparer<IDashStyle>.Default.Equals(DashStyle, other.DashStyle) && LineCap == other.LineCap && LineJoin == other.LineJoin)
		{
			return MiterLimit == other.MiterLimit;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Brush, Thickness, DashStyle, LineCap, LineJoin, MiterLimit).GetHashCode();
	}
}
