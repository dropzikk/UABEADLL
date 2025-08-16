using System;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Media;

public sealed class SolidColorBrush : Brush, ISolidColorBrush, IBrush, IMutableBrush
{
	public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<SolidColorBrush, Color>("Color");

	public Color Color
	{
		get
		{
			return GetValue(ColorProperty);
		}
		set
		{
			SetValue(ColorProperty, value);
		}
	}

	internal override Func<Compositor, ServerCompositionSimpleBrush> Factory => (Compositor c) => new ServerCompositionSimpleSolidColorBrush(c.Server);

	public SolidColorBrush()
	{
	}

	public SolidColorBrush(Color color, double opacity = 1.0)
	{
		Color = color;
		base.Opacity = opacity;
	}

	public SolidColorBrush(uint color)
		: this(Color.FromUInt32(color))
	{
	}

	public new static SolidColorBrush Parse(string s)
	{
		ISolidColorBrush solidColorBrush = (ISolidColorBrush)Brush.Parse(s);
		if (!(solidColorBrush is SolidColorBrush result))
		{
			return new SolidColorBrush(solidColorBrush.Color);
		}
		return result;
	}

	public override string ToString()
	{
		return Color.ToString();
	}

	public IImmutableBrush ToImmutable()
	{
		return new ImmutableSolidColorBrush(this);
	}

	private protected override void SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		base.SerializeChanges(c, writer);
		ServerCompositionSimpleSolidColorBrush.SerializeAllChanges(writer, Color);
	}
}
