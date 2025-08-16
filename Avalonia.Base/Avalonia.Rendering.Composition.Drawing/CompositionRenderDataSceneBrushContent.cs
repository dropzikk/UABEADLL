using System;
using Avalonia.Media;
using Avalonia.Platform;

namespace Avalonia.Rendering.Composition.Drawing;

internal class CompositionRenderDataSceneBrushContent : ISceneBrushContent, IImmutableBrush, IBrush, IDisposable
{
	private Rect? _rect;

	public CompositionRenderData RenderData { get; }

	public ITileBrush Brush { get; }

	public Rect Rect => _rect ?? (RenderData.Server?.Bounds).GetValueOrDefault();

	public double Opacity => Brush.Opacity;

	public ITransform? Transform => Brush.Transform;

	public RelativePoint TransformOrigin => Brush.TransformOrigin;

	public bool UseScalableRasterization { get; }

	public CompositionRenderDataSceneBrushContent(ITileBrush brush, CompositionRenderData renderData, Rect? rect, bool useScalableRasterization)
	{
		Brush = brush;
		_rect = rect;
		UseScalableRasterization = useScalableRasterization;
		RenderData = renderData;
	}

	public void Dispose()
	{
	}

	public void Render(IDrawingContextImpl context, Matrix? transform)
	{
		if (transform.HasValue)
		{
			Matrix transform2 = context.Transform;
			context.Transform = transform.Value * transform2;
			RenderData.Server.Render(context);
			context.Transform = transform2;
		}
		else
		{
			RenderData.Server.Render(context);
		}
	}
}
