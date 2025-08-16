using System;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Drawing;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Utilities;

namespace Avalonia.Media;

public sealed class DrawingBrush : TileBrush, ISceneBrush, ITileBrush, IBrush
{
	public static readonly StyledProperty<Drawing?> DrawingProperty = AvaloniaProperty.Register<DrawingBrush, Drawing>("Drawing");

	private InlineDictionary<Compositor, CompositionRenderDataSceneBrushContent?> _renderDataDictionary;

	public Drawing? Drawing
	{
		get
		{
			return GetValue(DrawingProperty);
		}
		set
		{
			SetValue(DrawingProperty, value);
		}
	}

	internal override Func<Compositor, ServerCompositionSimpleBrush> Factory => (Compositor c) => new ServerCompositionSimpleContentBrush(c.Server);

	public DrawingBrush()
	{
	}

	public DrawingBrush(Drawing visual)
	{
		Drawing = visual;
	}

	ISceneBrushContent? ISceneBrush.CreateContent()
	{
		if (Drawing == null)
		{
			return null;
		}
		using RenderDataDrawingContext renderDataDrawingContext = new RenderDataDrawingContext(null);
		Drawing?.Draw(renderDataDrawingContext);
		return renderDataDrawingContext.GetImmediateSceneBrushContent(this, null, useScalableRasterization: true);
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
		if (Drawing == null)
		{
			return null;
		}
		using RenderDataDrawingContext renderDataDrawingContext = new RenderDataDrawingContext(c);
		Drawing?.Draw(renderDataDrawingContext);
		CompositionRenderData renderResults = renderDataDrawingContext.GetRenderResults();
		if (renderResults == null)
		{
			return null;
		}
		return new CompositionRenderDataSceneBrushContent((ServerCompositionSimpleContentBrush)((ICompositionRenderResource<IBrush>)this).GetForCompositor(c), renderResults, null, useScalableRasterization: true);
	}
}
