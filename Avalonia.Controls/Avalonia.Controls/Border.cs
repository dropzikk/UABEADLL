using Avalonia.Controls.Utils;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

public class Border : Decorator, IVisualWithRoundRectClip
{
	public static readonly StyledProperty<IBrush?> BackgroundProperty;

	public static readonly StyledProperty<IBrush?> BorderBrushProperty;

	public static readonly StyledProperty<Thickness> BorderThicknessProperty;

	public static readonly StyledProperty<CornerRadius> CornerRadiusProperty;

	public static readonly StyledProperty<BoxShadows> BoxShadowProperty;

	private readonly BorderRenderHelper _borderRenderHelper = new BorderRenderHelper();

	private Thickness? _layoutThickness;

	private double _scale;

	private CompositionBorderVisual? _borderVisual;

	public IBrush? Background
	{
		get
		{
			return GetValue(BackgroundProperty);
		}
		set
		{
			SetValue(BackgroundProperty, value);
		}
	}

	public IBrush? BorderBrush
	{
		get
		{
			return GetValue(BorderBrushProperty);
		}
		set
		{
			SetValue(BorderBrushProperty, value);
		}
	}

	public Thickness BorderThickness
	{
		get
		{
			return GetValue(BorderThicknessProperty);
		}
		set
		{
			SetValue(BorderThicknessProperty, value);
		}
	}

	public CornerRadius CornerRadius
	{
		get
		{
			return GetValue(CornerRadiusProperty);
		}
		set
		{
			SetValue(CornerRadiusProperty, value);
		}
	}

	public BoxShadows BoxShadow
	{
		get
		{
			return GetValue(BoxShadowProperty);
		}
		set
		{
			SetValue(BoxShadowProperty, value);
		}
	}

	private Thickness LayoutThickness
	{
		get
		{
			VerifyScale();
			if (!_layoutThickness.HasValue)
			{
				Thickness thickness = BorderThickness;
				if (base.UseLayoutRounding)
				{
					thickness = LayoutHelper.RoundLayoutThickness(thickness, _scale, _scale);
				}
				_layoutThickness = thickness;
			}
			return _layoutThickness.Value;
		}
	}

	public CornerRadius ClipToBoundsRadius => CornerRadius;

	static Border()
	{
		BackgroundProperty = AvaloniaProperty.Register<Border, IBrush>("Background");
		BorderBrushProperty = AvaloniaProperty.Register<Border, IBrush>("BorderBrush");
		BorderThicknessProperty = AvaloniaProperty.Register<Border, Thickness>("BorderThickness");
		CornerRadiusProperty = AvaloniaProperty.Register<Border, CornerRadius>("CornerRadius");
		BoxShadowProperty = AvaloniaProperty.Register<Border, BoxShadows>("BoxShadow");
		Visual.AffectsRender<Border>(new AvaloniaProperty[5] { BackgroundProperty, BorderBrushProperty, BorderThicknessProperty, CornerRadiusProperty, BoxShadowProperty });
		Layoutable.AffectsMeasure<Border>(new AvaloniaProperty[1] { BorderThicknessProperty });
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		switch (change.Property.Name)
		{
		case "UseLayoutRounding":
		case "BorderThickness":
			_layoutThickness = null;
			break;
		case "CornerRadius":
			if (_borderVisual != null)
			{
				_borderVisual.CornerRadius = CornerRadius;
			}
			break;
		}
	}

	private void VerifyScale()
	{
		double layoutScale = LayoutHelper.GetLayoutScale(this);
		if (!MathUtilities.AreClose(layoutScale, _scale))
		{
			_scale = layoutScale;
			_layoutThickness = null;
		}
	}

	public sealed override void Render(DrawingContext context)
	{
		_borderRenderHelper.Render(context, base.Bounds.Size, LayoutThickness, CornerRadius, Background, BorderBrush, BoxShadow);
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		return LayoutHelper.MeasureChild(base.Child, availableSize, base.Padding, BorderThickness);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		return LayoutHelper.ArrangeChild(base.Child, finalSize, base.Padding, BorderThickness);
	}

	private protected override CompositionDrawListVisual CreateCompositionVisual(Compositor compositor)
	{
		CompositionBorderVisual obj = new CompositionBorderVisual(compositor, this)
		{
			CornerRadius = CornerRadius
		};
		CompositionBorderVisual result = obj;
		_borderVisual = obj;
		return result;
	}
}
