namespace Avalonia.Controls;

public class ColumnDefinition : DefinitionBase
{
	public static readonly StyledProperty<double> MaxWidthProperty;

	public static readonly StyledProperty<double> MinWidthProperty;

	public static readonly StyledProperty<GridLength> WidthProperty;

	public double ActualWidth => base.Parent?.GetFinalColumnDefinitionWidth(base.Index) ?? 0.0;

	public double MaxWidth
	{
		get
		{
			return GetValue(MaxWidthProperty);
		}
		set
		{
			SetValue(MaxWidthProperty, value);
		}
	}

	public double MinWidth
	{
		get
		{
			return GetValue(MinWidthProperty);
		}
		set
		{
			SetValue(MinWidthProperty, value);
		}
	}

	public GridLength Width
	{
		get
		{
			return GetValue(WidthProperty);
		}
		set
		{
			SetValue(WidthProperty, value);
		}
	}

	internal override GridLength UserSizeValueCache => Width;

	internal override double UserMinSizeValueCache => MinWidth;

	internal override double UserMaxSizeValueCache => MaxWidth;

	static ColumnDefinition()
	{
		MaxWidthProperty = AvaloniaProperty.Register<ColumnDefinition, double>("MaxWidth", double.PositiveInfinity);
		MinWidthProperty = AvaloniaProperty.Register<ColumnDefinition, double>("MinWidth", 0.0);
		WidthProperty = AvaloniaProperty.Register<ColumnDefinition, GridLength>("Width", new GridLength(1.0, GridUnitType.Star));
		DefinitionBase.AffectsParentMeasure(MinWidthProperty, MaxWidthProperty);
		WidthProperty.Changed.AddClassHandler<DefinitionBase>(DefinitionBase.OnUserSizePropertyChanged);
	}

	public ColumnDefinition()
	{
	}

	public ColumnDefinition(double value, GridUnitType type)
		: this(new GridLength(value, type))
	{
	}

	public ColumnDefinition(GridLength width)
	{
		Width = width;
	}
}
