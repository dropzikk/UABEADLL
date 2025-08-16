using System;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSimpleConicGradientBrush : ServerCompositionSimpleGradientBrush, IConicGradientBrush, IGradientBrush, IBrush
{
	private double _angle;

	internal static CompositionProperty s_IdOfAngleProperty = CompositionProperty.Register();

	private RelativePoint _center;

	internal static CompositionProperty s_IdOfCenterProperty = CompositionProperty.Register();

	public double Angle
	{
		get
		{
			return GetValue(s_IdOfAngleProperty, ref _angle);
		}
		set
		{
			bool flag = false;
			if (_angle != value)
			{
				flag = true;
			}
			SetValue(s_IdOfAngleProperty, ref _angle, value);
		}
	}

	public RelativePoint Center
	{
		get
		{
			return GetValue(s_IdOfCenterProperty, ref _center);
		}
		set
		{
			bool flag = false;
			if (_center != value)
			{
				flag = true;
			}
			SetValue(s_IdOfCenterProperty, ref _center, value);
		}
	}

	internal ServerCompositionSimpleConicGradientBrush(ServerCompositor compositor)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		CompositionSimpleConicGradientBrushChangedFields num = reader.Read<CompositionSimpleConicGradientBrushChangedFields>();
		if ((num & CompositionSimpleConicGradientBrushChangedFields.Angle) == CompositionSimpleConicGradientBrushChangedFields.Angle)
		{
			Angle = reader.Read<double>();
		}
		if ((num & CompositionSimpleConicGradientBrushChangedFields.Center) == CompositionSimpleConicGradientBrushChangedFields.Center)
		{
			Center = reader.Read<RelativePoint>();
		}
	}

	internal static void SerializeAllChanges(BatchStreamWriter writer, double angle, RelativePoint center)
	{
		writer.Write(CompositionSimpleConicGradientBrushChangedFields.Angle | CompositionSimpleConicGradientBrushChangedFields.Center);
		writer.Write(angle);
		writer.Write(center);
	}
}
