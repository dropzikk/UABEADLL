using System;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Media;

public sealed class LinearGradientBrush : GradientBrush, ILinearGradientBrush, IGradientBrush, IBrush
{
	public static readonly StyledProperty<RelativePoint> StartPointProperty = AvaloniaProperty.Register<LinearGradientBrush, RelativePoint>("StartPoint", RelativePoint.TopLeft);

	public static readonly StyledProperty<RelativePoint> EndPointProperty = AvaloniaProperty.Register<LinearGradientBrush, RelativePoint>("EndPoint", RelativePoint.BottomRight);

	public RelativePoint StartPoint
	{
		get
		{
			return GetValue(StartPointProperty);
		}
		set
		{
			SetValue(StartPointProperty, value);
		}
	}

	public RelativePoint EndPoint
	{
		get
		{
			return GetValue(EndPointProperty);
		}
		set
		{
			SetValue(EndPointProperty, value);
		}
	}

	internal override Func<Compositor, ServerCompositionSimpleBrush> Factory => (Compositor c) => new ServerCompositionSimpleLinearGradientBrush(c.Server);

	public override IImmutableBrush ToImmutable()
	{
		return new ImmutableLinearGradientBrush(this);
	}

	private protected override void SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		base.SerializeChanges(c, writer);
		ServerCompositionSimpleLinearGradientBrush.SerializeAllChanges(writer, StartPoint, EndPoint);
	}
}
