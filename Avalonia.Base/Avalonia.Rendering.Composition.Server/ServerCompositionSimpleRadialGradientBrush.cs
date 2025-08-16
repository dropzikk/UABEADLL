using System;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSimpleRadialGradientBrush : ServerCompositionSimpleGradientBrush, IRadialGradientBrush, IGradientBrush, IBrush
{
	private RelativePoint _center;

	internal static CompositionProperty s_IdOfCenterProperty = CompositionProperty.Register();

	private RelativePoint _gradientOrigin;

	internal static CompositionProperty s_IdOfGradientOriginProperty = CompositionProperty.Register();

	private double _radius;

	internal static CompositionProperty s_IdOfRadiusProperty = CompositionProperty.Register();

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

	public RelativePoint GradientOrigin
	{
		get
		{
			return GetValue(s_IdOfGradientOriginProperty, ref _gradientOrigin);
		}
		set
		{
			bool flag = false;
			if (_gradientOrigin != value)
			{
				flag = true;
			}
			SetValue(s_IdOfGradientOriginProperty, ref _gradientOrigin, value);
		}
	}

	public double Radius
	{
		get
		{
			return GetValue(s_IdOfRadiusProperty, ref _radius);
		}
		set
		{
			bool flag = false;
			if (_radius != value)
			{
				flag = true;
			}
			SetValue(s_IdOfRadiusProperty, ref _radius, value);
		}
	}

	internal ServerCompositionSimpleRadialGradientBrush(ServerCompositor compositor)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		CompositionSimpleRadialGradientBrushChangedFields num = reader.Read<CompositionSimpleRadialGradientBrushChangedFields>();
		if ((num & CompositionSimpleRadialGradientBrushChangedFields.Center) == CompositionSimpleRadialGradientBrushChangedFields.Center)
		{
			Center = reader.Read<RelativePoint>();
		}
		if ((num & CompositionSimpleRadialGradientBrushChangedFields.GradientOrigin) == CompositionSimpleRadialGradientBrushChangedFields.GradientOrigin)
		{
			GradientOrigin = reader.Read<RelativePoint>();
		}
		if ((num & CompositionSimpleRadialGradientBrushChangedFields.Radius) == CompositionSimpleRadialGradientBrushChangedFields.Radius)
		{
			Radius = reader.Read<double>();
		}
	}

	internal static void SerializeAllChanges(BatchStreamWriter writer, RelativePoint center, RelativePoint gradientOrigin, double radius)
	{
		writer.Write(CompositionSimpleRadialGradientBrushChangedFields.Center | CompositionSimpleRadialGradientBrushChangedFields.GradientOrigin | CompositionSimpleRadialGradientBrushChangedFields.Radius);
		writer.Write(center);
		writer.Write(gradientOrigin);
		writer.Write(radius);
	}
}
