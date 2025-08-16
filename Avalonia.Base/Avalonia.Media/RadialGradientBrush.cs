using System;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Media;

public sealed class RadialGradientBrush : GradientBrush, IRadialGradientBrush, IGradientBrush, IBrush
{
	public static readonly StyledProperty<RelativePoint> CenterProperty = AvaloniaProperty.Register<RadialGradientBrush, RelativePoint>("Center", RelativePoint.Center);

	public static readonly StyledProperty<RelativePoint> GradientOriginProperty = AvaloniaProperty.Register<RadialGradientBrush, RelativePoint>("GradientOrigin", RelativePoint.Center);

	public static readonly StyledProperty<double> RadiusProperty = AvaloniaProperty.Register<RadialGradientBrush, double>("Radius", 0.5);

	public RelativePoint Center
	{
		get
		{
			return GetValue(CenterProperty);
		}
		set
		{
			SetValue(CenterProperty, value);
		}
	}

	public RelativePoint GradientOrigin
	{
		get
		{
			return GetValue(GradientOriginProperty);
		}
		set
		{
			SetValue(GradientOriginProperty, value);
		}
	}

	public double Radius
	{
		get
		{
			return GetValue(RadiusProperty);
		}
		set
		{
			SetValue(RadiusProperty, value);
		}
	}

	internal override Func<Compositor, ServerCompositionSimpleBrush> Factory => (Compositor c) => new ServerCompositionSimpleRadialGradientBrush(c.Server);

	public override IImmutableBrush ToImmutable()
	{
		return new ImmutableRadialGradientBrush(this);
	}

	private protected override void SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		base.SerializeChanges(c, writer);
		ServerCompositionSimpleRadialGradientBrush.SerializeAllChanges(writer, Center, GradientOrigin, Radius);
	}
}
