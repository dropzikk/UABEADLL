using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Metadata;

namespace Avalonia.Controls;

public class Viewbox : Control
{
	private class ViewboxContainer : Control
	{
		private Control? _child;

		public Control? Child
		{
			get
			{
				return _child;
			}
			set
			{
				if (_child != value)
				{
					if (_child != null)
					{
						base.VisualChildren.Remove(_child);
					}
					_child = value;
					if (_child != null)
					{
						base.VisualChildren.Add(_child);
					}
					InvalidateMeasure();
				}
			}
		}
	}

	private readonly ViewboxContainer _containerVisual;

	public static readonly StyledProperty<Stretch> StretchProperty;

	public static readonly StyledProperty<StretchDirection> StretchDirectionProperty;

	public static readonly StyledProperty<Control?> ChildProperty;

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

	[Content]
	public Control? Child
	{
		get
		{
			return GetValue(ChildProperty);
		}
		set
		{
			SetValue(ChildProperty, value);
		}
	}

	internal ITransform? InternalTransform
	{
		get
		{
			return _containerVisual.RenderTransform;
		}
		set
		{
			_containerVisual.RenderTransform = value;
		}
	}

	static Viewbox()
	{
		StretchProperty = AvaloniaProperty.Register<Viewbox, Stretch>("Stretch", Stretch.Uniform);
		StretchDirectionProperty = AvaloniaProperty.Register<Viewbox, StretchDirection>("StretchDirection", StretchDirection.Both);
		ChildProperty = Decorator.ChildProperty.AddOwner<Viewbox>();
		Visual.ClipToBoundsProperty.OverrideDefaultValue<Viewbox>(defaultValue: true);
		Layoutable.UseLayoutRoundingProperty.OverrideDefaultValue<Viewbox>(defaultValue: true);
		Layoutable.AffectsMeasure<Viewbox>(new AvaloniaProperty[2] { StretchProperty, StretchDirectionProperty });
	}

	public Viewbox()
	{
		_containerVisual = new ViewboxContainer();
		_containerVisual.RenderTransformOrigin = RelativePoint.TopLeft;
		((ISetLogicalParent)_containerVisual).SetParent((ILogical?)this);
		base.VisualChildren.Add(_containerVisual);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ChildProperty)
		{
			var (control, control2) = change.GetOldAndNewValue<Control>();
			if (control != null)
			{
				((ISetLogicalParent)control).SetParent((ILogical?)null);
				base.LogicalChildren.Remove(control);
			}
			_containerVisual.Child = control2;
			if (control2 != null)
			{
				((ISetLogicalParent)control2).SetParent((ILogical?)this);
				base.LogicalChildren.Add(control2);
			}
			InvalidateMeasure();
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		ViewboxContainer containerVisual = _containerVisual;
		containerVisual.Measure(Size.Infinity);
		Size desiredSize = containerVisual.DesiredSize;
		return Stretch.CalculateSize(availableSize, desiredSize, StretchDirection);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		ViewboxContainer containerVisual = _containerVisual;
		Size desiredSize = containerVisual.DesiredSize;
		Vector vector = Stretch.CalculateScaling(finalSize, desiredSize, StretchDirection);
		InternalTransform = new ImmutableTransform(Matrix.CreateScale(vector.X, vector.Y));
		containerVisual.Arrange(new Rect(desiredSize));
		return desiredSize * vector;
	}
}
