using System;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSimplePen : SimpleServerRenderResource, IPen
{
	private IBrush? _brush;

	internal static CompositionProperty s_IdOfBrushProperty = CompositionProperty.Register();

	private ImmutableDashStyle? _dashStyle;

	internal static CompositionProperty s_IdOfDashStyleProperty = CompositionProperty.Register();

	private PenLineCap _lineCap;

	internal static CompositionProperty s_IdOfLineCapProperty = CompositionProperty.Register();

	private PenLineJoin _lineJoin;

	internal static CompositionProperty s_IdOfLineJoinProperty = CompositionProperty.Register();

	private double _miterLimit;

	internal static CompositionProperty s_IdOfMiterLimitProperty = CompositionProperty.Register();

	private double _thickness;

	internal static CompositionProperty s_IdOfThicknessProperty = CompositionProperty.Register();

	IDashStyle? IPen.DashStyle => DashStyle;

	public IBrush? Brush
	{
		get
		{
			return GetValue(s_IdOfBrushProperty, ref _brush);
		}
		set
		{
			bool flag = false;
			if (_brush != value)
			{
				flag = true;
			}
			SetValue(s_IdOfBrushProperty, ref _brush, value);
		}
	}

	public ImmutableDashStyle? DashStyle
	{
		get
		{
			return GetValue(s_IdOfDashStyleProperty, ref _dashStyle);
		}
		set
		{
			bool flag = false;
			if (_dashStyle != value)
			{
				flag = true;
			}
			SetValue(s_IdOfDashStyleProperty, ref _dashStyle, value);
		}
	}

	public PenLineCap LineCap
	{
		get
		{
			return GetValue(s_IdOfLineCapProperty, ref _lineCap);
		}
		set
		{
			bool flag = false;
			if (_lineCap != value)
			{
				flag = true;
			}
			SetValue(s_IdOfLineCapProperty, ref _lineCap, value);
		}
	}

	public PenLineJoin LineJoin
	{
		get
		{
			return GetValue(s_IdOfLineJoinProperty, ref _lineJoin);
		}
		set
		{
			bool flag = false;
			if (_lineJoin != value)
			{
				flag = true;
			}
			SetValue(s_IdOfLineJoinProperty, ref _lineJoin, value);
		}
	}

	public double MiterLimit
	{
		get
		{
			return GetValue(s_IdOfMiterLimitProperty, ref _miterLimit);
		}
		set
		{
			bool flag = false;
			if (_miterLimit != value)
			{
				flag = true;
			}
			SetValue(s_IdOfMiterLimitProperty, ref _miterLimit, value);
		}
	}

	public double Thickness
	{
		get
		{
			return GetValue(s_IdOfThicknessProperty, ref _thickness);
		}
		set
		{
			bool flag = false;
			if (_thickness != value)
			{
				flag = true;
			}
			SetValue(s_IdOfThicknessProperty, ref _thickness, value);
		}
	}

	internal ServerCompositionSimplePen(ServerCompositor compositor)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		CompositionSimplePenChangedFields num = reader.Read<CompositionSimplePenChangedFields>();
		if ((num & CompositionSimplePenChangedFields.Brush) == CompositionSimplePenChangedFields.Brush)
		{
			Brush = reader.ReadObject<IBrush>();
		}
		if ((num & CompositionSimplePenChangedFields.DashStyle) == CompositionSimplePenChangedFields.DashStyle)
		{
			DashStyle = reader.ReadObject<ImmutableDashStyle>();
		}
		if ((num & CompositionSimplePenChangedFields.LineCap) == CompositionSimplePenChangedFields.LineCap)
		{
			LineCap = reader.Read<PenLineCap>();
		}
		if ((num & CompositionSimplePenChangedFields.LineJoin) == CompositionSimplePenChangedFields.LineJoin)
		{
			LineJoin = reader.Read<PenLineJoin>();
		}
		if ((num & CompositionSimplePenChangedFields.MiterLimit) == CompositionSimplePenChangedFields.MiterLimit)
		{
			MiterLimit = reader.Read<double>();
		}
		if ((num & CompositionSimplePenChangedFields.Thickness) == CompositionSimplePenChangedFields.Thickness)
		{
			Thickness = reader.Read<double>();
		}
	}

	internal static void SerializeAllChanges(BatchStreamWriter writer, IBrush? brush, ImmutableDashStyle? dashStyle, PenLineCap lineCap, PenLineJoin lineJoin, double miterLimit, double thickness)
	{
		writer.Write(CompositionSimplePenChangedFields.Brush | CompositionSimplePenChangedFields.DashStyle | CompositionSimplePenChangedFields.LineCap | CompositionSimplePenChangedFields.LineJoin | CompositionSimplePenChangedFields.MiterLimit | CompositionSimplePenChangedFields.Thickness);
		writer.WriteObject(brush);
		writer.WriteObject(dashStyle);
		writer.Write(lineCap);
		writer.Write(lineJoin);
		writer.Write(miterLimit);
		writer.Write(thickness);
	}
}
