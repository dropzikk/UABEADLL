using System;
using Avalonia.Media;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionContainerVisual : ServerCompositionVisual
{
	private Rect? _transformedContentBounds;

	private IImmutableEffect? _oldEffect;

	public ServerCompositionVisualCollection Children { get; private set; }

	protected override void RenderCore(CompositorDrawingContextProxy canvas, Rect currentTransformedClip)
	{
		base.RenderCore(canvas, currentTransformedClip);
		foreach (ServerCompositionVisual child in Children)
		{
			child.Render(canvas, currentTransformedClip);
		}
	}

	public override UpdateResult Update(ServerCompositionTarget root)
	{
		var (rect2, flag3, flag4) = (UpdateResult)(ref base.Update(root));
		foreach (ServerCompositionVisual child in Children)
		{
			if (child.AdornedVisual != null)
			{
				root.EnqueueAdornerUpdate(child);
				continue;
			}
			UpdateResult updateResult2 = child.Update(root);
			flag3 |= updateResult2.InvalidatedOld;
			flag4 |= updateResult2.InvalidatedNew;
			rect2 = Rect.Union(rect2, updateResult2.Bounds);
		}
		if (!base.Effect.EffectEquals(_oldEffect))
		{
			flag3 = (flag4 = true);
		}
		if (_oldEffect != null && flag3 && _transformedContentBounds.HasValue)
		{
			AddEffectPaddedDirtyRect(_oldEffect, _transformedContentBounds.Value);
		}
		if (base.Effect != null && flag4 && rect2.HasValue)
		{
			AddEffectPaddedDirtyRect(base.Effect, rect2.Value);
		}
		_oldEffect = base.Effect;
		_transformedContentBounds = rect2;
		IsDirtyComposition = false;
		return new UpdateResult(_transformedContentBounds, flag3, flag4);
	}

	private void AddEffectPaddedDirtyRect(IImmutableEffect effect, Rect transformedBounds)
	{
		Thickness thickness = effect.GetEffectOutputPadding();
		if (thickness == default(Thickness))
		{
			AddDirtyRect(transformedBounds);
			return;
		}
		Matrix combinedTransformMatrix = base.CombinedTransformMatrix;
		if (combinedTransformMatrix.M12 == 0.0 && combinedTransformMatrix.M13 == 0.0 && combinedTransformMatrix.M21 == 0.0 && combinedTransformMatrix.M23 == 0.0 && combinedTransformMatrix.M31 == 0.0 && combinedTransformMatrix.M32 == 0.0)
		{
			thickness = new Thickness(thickness.Left * base.CombinedTransformMatrix.M11, thickness.Top * base.CombinedTransformMatrix.M22, thickness.Right * base.CombinedTransformMatrix.M11, thickness.Bottom * base.CombinedTransformMatrix.M22);
		}
		else
		{
			Rect rect = default(Rect).Inflate(thickness).TransformToAABB(base.CombinedTransformMatrix);
			thickness = new Thickness(Math.Max(rect.Width, rect.Height));
		}
		AddDirtyRect(transformedBounds.Inflate(thickness));
	}

	internal ServerCompositionContainerVisual(ServerCompositor compositor)
		: base(compositor)
	{
		Initialize();
	}

	private void Initialize()
	{
		Children = new ServerCompositionVisualCollection(base.Compositor);
	}
}
