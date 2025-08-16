using System;
using Avalonia.Platform;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSurfaceVisual : ServerCompositionContainerVisual
{
	private ServerCompositionSurface? _surface;

	internal static CompositionProperty s_IdOfSurfaceProperty = CompositionProperty.Register();

	public ServerCompositionSurface? Surface
	{
		get
		{
			return GetValue(s_IdOfSurfaceProperty, ref _surface);
		}
		set
		{
			bool flag = false;
			if (_surface != value)
			{
				OnSurfaceChanging();
				flag = true;
			}
			SetValue(s_IdOfSurfaceProperty, ref _surface, value);
			if (flag)
			{
				OnSurfaceChanged();
			}
		}
	}

	protected override void RenderCore(CompositorDrawingContextProxy canvas, Rect currentTransformedClip)
	{
		if (Surface != null && Surface.Bitmap != null)
		{
			IBitmapImpl item = Surface.Bitmap.Item;
			canvas.DrawBitmap(Surface.Bitmap.Item, 1.0, new Rect(item.PixelSize.ToSize(1.0)), new Rect(new Size(base.Size.X, base.Size.Y)));
		}
	}

	private void OnSurfaceInvalidated()
	{
		ValuesInvalidated();
	}

	protected override void OnAttachedToRoot(ServerCompositionTarget target)
	{
		if (Surface != null)
		{
			ServerCompositionSurface? surface = Surface;
			surface.Changed = (Action)Delegate.Combine(surface.Changed, new Action(OnSurfaceInvalidated));
		}
		base.OnAttachedToRoot(target);
	}

	protected override void OnDetachedFromRoot(ServerCompositionTarget target)
	{
		if (Surface != null)
		{
			ServerCompositionSurface? surface = Surface;
			surface.Changed = (Action)Delegate.Remove(surface.Changed, new Action(OnSurfaceInvalidated));
		}
		base.OnDetachedFromRoot(target);
	}

	internal ServerCompositionSurfaceVisual(ServerCompositor compositor)
		: base(compositor)
	{
	}

	private void OnSurfaceChanged()
	{
		if (Surface != null)
		{
			ServerCompositionSurface? surface = Surface;
			surface.Changed = (Action)Delegate.Combine(surface.Changed, new Action(OnSurfaceInvalidated));
		}
	}

	private void OnSurfaceChanging()
	{
		if (Surface != null)
		{
			ServerCompositionSurface? surface = Surface;
			surface.Changed = (Action)Delegate.Remove(surface.Changed, new Action(OnSurfaceInvalidated));
		}
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		if ((reader.Read<CompositionSurfaceVisualChangedFields>() & CompositionSurfaceVisualChangedFields.Surface) == CompositionSurfaceVisualChangedFields.Surface)
		{
			Surface = reader.ReadObject<ServerCompositionSurface>();
		}
	}
}
