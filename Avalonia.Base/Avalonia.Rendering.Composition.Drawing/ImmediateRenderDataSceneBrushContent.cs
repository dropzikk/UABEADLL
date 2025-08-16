using System;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.Composition.Drawing.Nodes;
using Avalonia.Threading;

namespace Avalonia.Rendering.Composition.Drawing;

internal class ImmediateRenderDataSceneBrushContent : ISceneBrushContent, IImmutableBrush, IBrush, IDisposable
{
	private List<IRenderDataItem>? _items;

	private readonly ThreadSafeObjectPool<List<IRenderDataItem>> _pool;

	public ITileBrush Brush { get; }

	public Rect Rect { get; }

	public double Opacity => Brush.Opacity;

	public ITransform? Transform => Brush.Transform;

	public RelativePoint TransformOrigin => Brush.TransformOrigin;

	public bool UseScalableRasterization { get; }

	public ImmediateRenderDataSceneBrushContent(ITileBrush brush, List<IRenderDataItem> items, Rect? rect, bool useScalableRasterization, ThreadSafeObjectPool<List<IRenderDataItem>> pool)
	{
		Brush = brush;
		_items = items;
		_pool = pool;
		UseScalableRasterization = useScalableRasterization;
		if (!rect.HasValue)
		{
			foreach (IRenderDataItem item in _items)
			{
				rect = Rect.Union(rect, item.Bounds);
			}
			rect = ServerCompositionRenderData.ApplyRenderBoundsRounding(rect);
		}
		Rect = rect.GetValueOrDefault();
	}

	public void Dispose()
	{
		if (_items == null)
		{
			return;
		}
		foreach (IRenderDataItem item in _items)
		{
			(item as IDisposable)?.Dispose();
		}
		_items.Clear();
		_pool.ReturnAndSetNull(ref _items);
	}

	private void Render(IDrawingContextImpl context)
	{
		if (_items == null)
		{
			return;
		}
		RenderDataNodeRenderContext context2 = new RenderDataNodeRenderContext(context);
		try
		{
			foreach (IRenderDataItem item in _items)
			{
				item.Invoke(ref context2);
			}
		}
		finally
		{
			context2.Dispose();
		}
	}

	public void Render(IDrawingContextImpl context, Matrix? transform)
	{
		if (transform.HasValue)
		{
			Matrix transform2 = context.Transform;
			context.Transform = transform.Value * transform2;
			Render(context);
			context.Transform = transform2;
		}
		else
		{
			Render(context);
		}
	}
}
