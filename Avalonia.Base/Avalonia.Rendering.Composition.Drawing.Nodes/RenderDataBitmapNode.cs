using System;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataBitmapNode : IRenderDataItem, IDisposable
{
	public IRef<IBitmapImpl>? Bitmap { get; set; }

	public double Opacity { get; set; }

	public Rect SourceRect { get; set; }

	public Rect DestRect { get; set; }

	public Rect? Bounds => DestRect;

	public bool HitTest(Point p)
	{
		return DestRect.Contains(p);
	}

	public void Invoke(ref RenderDataNodeRenderContext context)
	{
		if (Bitmap != null)
		{
			context.Context.DrawBitmap(Bitmap.Item, Opacity, SourceRect, DestRect);
		}
	}

	public void Dispose()
	{
		Bitmap?.Dispose();
		Bitmap = null;
	}
}
