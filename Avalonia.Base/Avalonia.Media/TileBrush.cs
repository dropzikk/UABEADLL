using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Media;

public abstract class TileBrush : Brush, ITileBrush, IBrush
{
	public static readonly StyledProperty<AlignmentX> AlignmentXProperty = AvaloniaProperty.Register<TileBrush, AlignmentX>("AlignmentX", AlignmentX.Center);

	public static readonly StyledProperty<AlignmentY> AlignmentYProperty = AvaloniaProperty.Register<TileBrush, AlignmentY>("AlignmentY", AlignmentY.Center);

	public static readonly StyledProperty<RelativeRect> DestinationRectProperty = AvaloniaProperty.Register<TileBrush, RelativeRect>("DestinationRect", RelativeRect.Fill);

	public static readonly StyledProperty<RelativeRect> SourceRectProperty = AvaloniaProperty.Register<TileBrush, RelativeRect>("SourceRect", RelativeRect.Fill);

	public static readonly StyledProperty<Stretch> StretchProperty = AvaloniaProperty.Register<TileBrush, Stretch>("Stretch", Stretch.Uniform);

	public static readonly StyledProperty<TileMode> TileModeProperty = AvaloniaProperty.Register<TileBrush, TileMode>("TileMode", TileMode.None);

	public AlignmentX AlignmentX
	{
		get
		{
			return GetValue(AlignmentXProperty);
		}
		set
		{
			SetValue(AlignmentXProperty, value);
		}
	}

	public AlignmentY AlignmentY
	{
		get
		{
			return GetValue(AlignmentYProperty);
		}
		set
		{
			SetValue(AlignmentYProperty, value);
		}
	}

	public RelativeRect DestinationRect
	{
		get
		{
			return GetValue(DestinationRectProperty);
		}
		set
		{
			SetValue(DestinationRectProperty, value);
		}
	}

	public RelativeRect SourceRect
	{
		get
		{
			return GetValue(SourceRectProperty);
		}
		set
		{
			SetValue(SourceRectProperty, value);
		}
	}

	public Stretch Stretch
	{
		get
		{
			return GetValue(StretchProperty);
		}
		set
		{
			SetValue(StretchProperty, value);
		}
	}

	public TileMode TileMode
	{
		get
		{
			return GetValue(TileModeProperty);
		}
		set
		{
			SetValue(TileModeProperty, value);
		}
	}

	internal TileBrush()
	{
	}

	private protected override void SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		base.SerializeChanges(c, writer);
		ServerCompositionSimpleTileBrush.SerializeAllChanges(writer, AlignmentX, AlignmentY, DestinationRect, SourceRect, Stretch, TileMode);
	}
}
