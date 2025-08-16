using System;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Utilities;

namespace Avalonia.Media;

public sealed class ImageBrush : TileBrush, IImageBrush, ITileBrush, IBrush, IMutableBrush
{
	public static readonly StyledProperty<IImageBrushSource?> SourceProperty = AvaloniaProperty.Register<ImageBrush, IImageBrushSource>("Source");

	public IImageBrushSource? Source
	{
		get
		{
			return GetValue(SourceProperty);
		}
		set
		{
			SetValue(SourceProperty, value);
		}
	}

	internal override Func<Compositor, ServerCompositionSimpleBrush> Factory => (Compositor c) => new ServerCompositionSimpleImageBrush(c.Server);

	public ImageBrush()
	{
	}

	public ImageBrush(IImageBrushSource? source)
	{
		Source = source;
	}

	public IImmutableBrush ToImmutable()
	{
		return new ImmutableImageBrush(this);
	}

	private protected override void SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		base.SerializeChanges(c, writer);
		IRef<IBitmapImpl> item = Source?.Bitmap?.Clone();
		writer.WriteObject(item);
	}
}
