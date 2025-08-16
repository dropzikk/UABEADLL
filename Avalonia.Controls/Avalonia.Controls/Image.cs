using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Avalonia.Controls;

public class Image : Control
{
	public static readonly StyledProperty<IImage?> SourceProperty;

	public static readonly StyledProperty<Stretch> StretchProperty;

	public static readonly StyledProperty<StretchDirection> StretchDirectionProperty;

	[Content]
	public IImage? Source
	{
		get
		{
			return GetValue(SourceProperty);
		}
		set
		{
			SetValue(SourceProperty, value);
		}
	}

	public Stretch Stretch
	{
		get
		{
			return GetValue(StretchProperty);
		}
		set
		{
			SetValue(StretchProperty, value);
		}
	}

	public StretchDirection StretchDirection
	{
		get
		{
			return GetValue(StretchDirectionProperty);
		}
		set
		{
			SetValue(StretchDirectionProperty, value);
		}
	}

	protected override bool BypassFlowDirectionPolicies => true;

	static Image()
	{
		SourceProperty = AvaloniaProperty.Register<Image, IImage>("Source");
		StretchProperty = AvaloniaProperty.Register<Image, Stretch>("Stretch", Stretch.Uniform);
		StretchDirectionProperty = AvaloniaProperty.Register<Image, StretchDirection>("StretchDirection", StretchDirection.Both);
		Visual.AffectsRender<Image>(new AvaloniaProperty[3] { SourceProperty, StretchProperty, StretchDirectionProperty });
		Layoutable.AffectsMeasure<Image>(new AvaloniaProperty[3] { SourceProperty, StretchProperty, StretchDirectionProperty });
		AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<Image>(AutomationControlType.Image);
	}

	public sealed override void Render(DrawingContext context)
	{
		IImage source = Source;
		if (source != null && base.Bounds.Width > 0.0 && base.Bounds.Height > 0.0)
		{
			Rect rect = new Rect(base.Bounds.Size);
			Size size = source.Size;
			Vector vector = Stretch.CalculateScaling(base.Bounds.Size, size, StretchDirection);
			Size size2 = size * vector;
			Rect destRect = rect.CenterRect(new Rect(size2)).Intersect(rect);
			Rect sourceRect = new Rect(size).CenterRect(new Rect(destRect.Size / vector));
			context.DrawImage(source, sourceRect, destRect);
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		IImage source = Source;
		Size result = default(Size);
		if (source != null)
		{
			return Stretch.CalculateSize(availableSize, source.Size, StretchDirection);
		}
		return result;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		IImage source = Source;
		if (source != null)
		{
			Size size = source.Size;
			return Stretch.CalculateSize(finalSize, size);
		}
		return default(Size);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ImageAutomationPeer(this);
	}
}
