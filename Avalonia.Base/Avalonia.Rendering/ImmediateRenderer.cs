using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Avalonia.Rendering;

internal class ImmediateRenderer
{
	public static void Render(Visual visual, DrawingContext context)
	{
		Render(context, visual, visual.Bounds);
	}

	private static Rect GetTransformedBounds(Visual visual)
	{
		if (visual.RenderTransform == null)
		{
			return visual.Bounds;
		}
		Point point = visual.RenderTransformOrigin.ToPixels(new Size(visual.Bounds.Width, visual.Bounds.Height));
		Matrix matrix = Matrix.CreateTranslation(visual.Bounds.Position + point);
		Matrix matrix2 = -matrix * visual.RenderTransform.Value * matrix;
		return visual.Bounds.TransformToAABB(matrix2);
	}

	public static void Render(DrawingContext context, Visual visual, Rect clipRect)
	{
		RenderOptions renderOptions = default(RenderOptions);
		PlatformDrawingContext platformDrawingContext = context as PlatformDrawingContext;
		try
		{
			if (platformDrawingContext != null)
			{
				renderOptions = platformDrawingContext.RenderOptions;
				platformDrawingContext.RenderOptions = visual.RenderOptions.MergeWith(platformDrawingContext.RenderOptions);
			}
			double opacity = visual.Opacity;
			bool clipToBounds = visual.ClipToBounds;
			Rect bounds = new Rect(visual.Bounds.Size);
			if (!visual.IsVisible || !(opacity > 0.0))
			{
				return;
			}
			Matrix matrix = Matrix.CreateTranslation(visual.Bounds.Position);
			Matrix identity = Matrix.Identity;
			if (visual.HasMirrorTransform)
			{
				Matrix matrix2 = new Matrix(-1.0, 0.0, 0.0, 1.0, visual.Bounds.Width, 0.0);
				identity *= matrix2;
			}
			if (visual.RenderTransform != null)
			{
				Matrix matrix3 = Matrix.CreateTranslation(visual.RenderTransformOrigin.ToPixels(new Size(visual.Bounds.Width, visual.Bounds.Height)));
				Matrix matrix4 = -matrix3 * visual.RenderTransform.Value * matrix3;
				identity *= matrix4;
			}
			matrix = identity * matrix;
			if (clipToBounds)
			{
				clipRect = ((visual.RenderTransform == null) ? clipRect.Intersect(new Rect(visual.Bounds.Size)) : new Rect(visual.Bounds.Size));
			}
			using (context.PushTransform(matrix))
			{
				using (context.PushOpacity(opacity))
				{
					using ((!clipToBounds) ? default(DrawingContext.PushedState) : ((visual is IVisualWithRoundRectClip visualWithRoundRectClip) ? context.PushClip(new RoundedRect(in bounds, visualWithRoundRectClip.ClipToBoundsRadius)) : context.PushClip(bounds)))
					{
						using ((visual.Clip != null) ? context.PushGeometryClip(visual.Clip) : default(DrawingContext.PushedState))
						{
							using ((visual.OpacityMask != null) ? context.PushOpacityMask(visual.OpacityMask, bounds) : default(DrawingContext.PushedState))
							{
								using (context.PushTransform(Matrix.Identity))
								{
									visual.Render(context);
									IEnumerable<Visual> enumerable;
									if (!visual.HasNonUniformZIndexChildren)
									{
										IEnumerable<Visual> visualChildren = visual.VisualChildren;
										enumerable = visualChildren;
									}
									else
									{
										IEnumerable<Visual> visualChildren = visual.VisualChildren.OrderBy((Visual x) => x, ZIndexComparer.Instance);
										enumerable = visualChildren;
									}
									foreach (Visual item in enumerable)
									{
										Rect transformedBounds = GetTransformedBounds(item);
										if (!item.ClipToBounds || clipRect.Intersects(transformedBounds))
										{
											Rect clipRect2 = ((item.RenderTransform == null) ? clipRect.Translate(-transformedBounds.Position) : clipRect);
											Render(context, item, clipRect2);
										}
									}
								}
							}
						}
					}
				}
			}
		}
		finally
		{
			if (platformDrawingContext != null)
			{
				platformDrawingContext.RenderOptions = renderOptions;
			}
		}
	}
}
