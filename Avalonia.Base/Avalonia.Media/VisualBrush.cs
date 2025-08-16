using System;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Drawing;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Utilities;

namespace Avalonia.Media;

public sealed class VisualBrush : TileBrush, ISceneBrush, ITileBrush, IBrush
{
	public static readonly StyledProperty<Visual?> VisualProperty = AvaloniaProperty.Register<VisualBrush, Visual>("Visual");

	private InlineDictionary<Compositor, CompositionRenderDataSceneBrushContent?> _renderDataDictionary;

	public Visual? Visual
	{
		get
		{
			return GetValue(VisualProperty);
		}
		set
		{
			SetValue(VisualProperty, value);
		}
	}

	internal override Func<Compositor, ServerCompositionSimpleBrush> Factory => (Compositor c) => new ServerCompositionSimpleContentBrush(c.Server);

	public VisualBrush()
	{
	}

	public VisualBrush(Visual visual)
	{
		Visual = visual;
	}

	ISceneBrushContent? ISceneBrush.CreateContent()
	{
		if (Visual == null)
		{
			return null;
		}
		if (Visual is IVisualBrushInitialize visualBrushInitialize)
		{
			visualBrushInitialize.EnsureInitialized();
		}
		using RenderDataDrawingContext renderDataDrawingContext = new RenderDataDrawingContext(null);
		ImmediateRenderer.Render(renderDataDrawingContext, Visual, Visual.Bounds);
		return renderDataDrawingContext.GetImmediateSceneBrushContent(this, new Rect(Visual.Bounds.Size), useScalableRasterization: false);
	}

	private protected override void OnReferencedFromCompositor(Compositor c)
	{
		_renderDataDictionary.Add(c, CreateServerContent(c));
		base.OnReferencedFromCompositor(c);
	}

	protected override void OnUnreferencedFromCompositor(Compositor c)
	{
		if (_renderDataDictionary.TryGetAndRemoveValue(c, out CompositionRenderDataSceneBrushContent value))
		{
			value?.RenderData.Dispose();
		}
		base.OnUnreferencedFromCompositor(c);
	}

	private protected override void SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		base.SerializeChanges(c, writer);
		if (_renderDataDictionary.TryGetValue(c, out CompositionRenderDataSceneBrushContent value))
		{
			writer.WriteObject(value);
		}
		else
		{
			writer.WriteObject(null);
		}
	}

	private CompositionRenderDataSceneBrushContent? CreateServerContent(Compositor c)
	{
		if (Visual == null)
		{
			return null;
		}
		if (Visual is IVisualBrushInitialize visualBrushInitialize)
		{
			visualBrushInitialize.EnsureInitialized();
		}
		using RenderDataDrawingContext renderDataDrawingContext = new RenderDataDrawingContext(c);
		ImmediateRenderer.Render(renderDataDrawingContext, Visual, Visual.Bounds);
		CompositionRenderData renderResults = renderDataDrawingContext.GetRenderResults();
		if (renderResults == null)
		{
			return null;
		}
		return new CompositionRenderDataSceneBrushContent((ServerCompositionSimpleContentBrush)((ICompositionRenderResource<IBrush>)this).GetForCompositor(c), renderResults, new Rect(Visual.Bounds.Size), useScalableRasterization: false);
	}
}
