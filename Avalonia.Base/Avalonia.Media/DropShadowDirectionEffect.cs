using System;

namespace Avalonia.Media;

public sealed class DropShadowDirectionEffect : DropShadowEffectBase, IDirectionDropShadowEffect, IDropShadowEffect, IEffect, IMutableEffect
{
	public static readonly StyledProperty<double> ShadowDepthProperty = AvaloniaProperty.Register<DropShadowDirectionEffect, double>("ShadowDepth", 5.0);

	public static readonly StyledProperty<double> DirectionProperty = AvaloniaProperty.Register<DropShadowDirectionEffect, double>("Direction", 315.0);

	public double ShadowDepth
	{
		get
		{
			return GetValue(ShadowDepthProperty);
		}
		set
		{
			SetValue(ShadowDepthProperty, value);
		}
	}

	public double Direction
	{
		get
		{
			return GetValue(DirectionProperty);
		}
		set
		{
			SetValue(DirectionProperty, value);
		}
	}

	public double OffsetX => Math.Cos(Direction * Math.PI / 180.0) * ShadowDepth;

	public double OffsetY => Math.Sin(Direction * Math.PI / 180.0) * ShadowDepth;

	public IImmutableEffect ToImmutable()
	{
		return new ImmutableDropShadowDirectionEffect(OffsetX, OffsetY, base.BlurRadius, base.Color, base.Opacity);
	}
}
