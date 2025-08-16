using System;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Media;

public sealed class ConicGradientBrush : GradientBrush, IConicGradientBrush, IGradientBrush, IBrush
{
	public static readonly StyledProperty<RelativePoint> CenterProperty = AvaloniaProperty.Register<ConicGradientBrush, RelativePoint>("Center", RelativePoint.Center);

	public static readonly StyledProperty<double> AngleProperty = AvaloniaProperty.Register<ConicGradientBrush, double>("Angle", 0.0);

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

	public double Angle
	{
		get
		{
			return GetValue(AngleProperty);
		}
		set
		{
			SetValue(AngleProperty, value);
		}
	}

	internal override Func<Compositor, ServerCompositionSimpleBrush> Factory => (Compositor c) => new ServerCompositionSimpleConicGradientBrush(c.Server);

	public override IImmutableBrush ToImmutable()
	{
		return new ImmutableConicGradientBrush(this);
	}

	private protected override void SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		base.SerializeChanges(c, writer);
		ServerCompositionSimpleConicGradientBrush.SerializeAllChanges(writer, Angle, Center);
	}
}
