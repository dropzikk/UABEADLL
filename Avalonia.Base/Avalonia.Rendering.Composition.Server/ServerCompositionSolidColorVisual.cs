using System;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSolidColorVisual : ServerCompositionContainerVisual
{
	private Color _color;

	internal static CompositionProperty s_IdOfColorProperty = CompositionProperty.Register();

	public Color Color
	{
		get
		{
			return GetAnimatedValue(s_IdOfColorProperty, ref _color);
		}
		set
		{
			SetAnimatedValue(s_IdOfColorProperty, out _color, value);
		}
	}

	protected override void RenderCore(CompositorDrawingContextProxy canvas, Rect currentTransformedClip)
	{
		canvas.DrawRectangle(new ImmutableSolidColorBrush(Color), null, new Rect(0.0, 0.0, base.Size.X, base.Size.Y));
	}

	internal ServerCompositionSolidColorVisual(ServerCompositor compositor)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		CompositionSolidColorVisualChangedFields compositionSolidColorVisualChangedFields = reader.Read<CompositionSolidColorVisualChangedFields>();
		if ((compositionSolidColorVisualChangedFields & CompositionSolidColorVisualChangedFields.ColorAnimated) == CompositionSolidColorVisualChangedFields.ColorAnimated)
		{
			SetAnimatedValue(s_IdOfColorProperty, ref _color, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionSolidColorVisualChangedFields & CompositionSolidColorVisualChangedFields.Color) == CompositionSolidColorVisualChangedFields.Color)
		{
			Color = reader.Read<Color>();
		}
	}

	public override ExpressionVariant GetPropertyForAnimation(string name)
	{
		if (name == "Color")
		{
			return Color;
		}
		return base.GetPropertyForAnimation(name);
	}

	public override CompositionProperty? GetCompositionProperty(string name)
	{
		if (name == "Color")
		{
			return s_IdOfColorProperty;
		}
		return base.GetCompositionProperty(name);
	}
}
