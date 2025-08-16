using System;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionExperimentalAcrylicVisual : ServerCompositionDrawListVisual
{
	private ImmutableExperimentalAcrylicMaterial _material;

	internal static CompositionProperty s_IdOfMaterialProperty = CompositionProperty.Register();

	private CornerRadius _cornerRadius;

	internal static CompositionProperty s_IdOfCornerRadiusProperty = CompositionProperty.Register();

	public override Rect OwnContentBounds => new Rect(0.0, 0.0, base.Size.X, base.Size.Y);

	public ImmutableExperimentalAcrylicMaterial Material
	{
		get
		{
			return GetValue(s_IdOfMaterialProperty, ref _material);
		}
		set
		{
			bool flag = false;
			if (_material != value)
			{
				flag = true;
			}
			SetValue(s_IdOfMaterialProperty, ref _material, value);
		}
	}

	public CornerRadius CornerRadius
	{
		get
		{
			return GetValue(s_IdOfCornerRadiusProperty, ref _cornerRadius);
		}
		set
		{
			bool flag = false;
			if (_cornerRadius != value)
			{
				flag = true;
			}
			SetValue(s_IdOfCornerRadiusProperty, ref _cornerRadius, value);
		}
	}

	protected override void RenderCore(CompositorDrawingContextProxy canvas, Rect currentTransformedClip)
	{
		CornerRadius cornerRadius = CornerRadius;
		canvas.DrawRectangle(Material, new RoundedRect(new Rect(0.0, 0.0, base.Size.X, base.Size.Y), cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft));
		base.RenderCore(canvas, currentTransformedClip);
	}

	public ServerCompositionExperimentalAcrylicVisual(ServerCompositor compositor, Visual v)
		: base(compositor, v)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		CompositionExperimentalAcrylicVisualChangedFields num = reader.Read<CompositionExperimentalAcrylicVisualChangedFields>();
		if ((num & CompositionExperimentalAcrylicVisualChangedFields.Material) == CompositionExperimentalAcrylicVisualChangedFields.Material)
		{
			Material = reader.Read<ImmutableExperimentalAcrylicMaterial>();
		}
		if ((num & CompositionExperimentalAcrylicVisualChangedFields.CornerRadius) == CompositionExperimentalAcrylicVisualChangedFields.CornerRadius)
		{
			CornerRadius = reader.Read<CornerRadius>();
		}
	}
}
