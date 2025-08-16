using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace Avalonia.Diagnostics.Controls;

internal class ThicknessEditor : ContentControl
{
	public static readonly StyledProperty<Thickness> ThicknessProperty = AvaloniaProperty.Register<ThicknessEditor, Thickness>("Thickness", default(Thickness), inherits: false, BindingMode.TwoWay);

	public static readonly StyledProperty<string?> HeaderProperty = AvaloniaProperty.Register<ThicknessEditor, string>("Header");

	public static readonly StyledProperty<bool> IsPresentProperty = AvaloniaProperty.Register<ThicknessEditor, bool>("IsPresent", defaultValue: true);

	public static readonly StyledProperty<double> LeftProperty = AvaloniaProperty.Register<ThicknessEditor, double>("Left", 0.0);

	public static readonly StyledProperty<double> TopProperty = AvaloniaProperty.Register<ThicknessEditor, double>("Top", 0.0);

	public static readonly StyledProperty<double> RightProperty = AvaloniaProperty.Register<ThicknessEditor, double>("Right", 0.0);

	public static readonly StyledProperty<double> BottomProperty = AvaloniaProperty.Register<ThicknessEditor, double>("Bottom", 0.0);

	public static readonly StyledProperty<IBrush> HighlightProperty = AvaloniaProperty.Register<ThicknessEditor, IBrush>("Highlight");

	private bool _isUpdatingThickness;

	public Thickness Thickness
	{
		get
		{
			return GetValue(ThicknessProperty);
		}
		set
		{
			SetValue(ThicknessProperty, value);
		}
	}

	public string? Header
	{
		get
		{
			return GetValue(HeaderProperty);
		}
		set
		{
			SetValue(HeaderProperty, value);
		}
	}

	public bool IsPresent
	{
		get
		{
			return GetValue(IsPresentProperty);
		}
		set
		{
			SetValue(IsPresentProperty, value);
		}
	}

	public double Left
	{
		get
		{
			return GetValue(LeftProperty);
		}
		set
		{
			SetValue(LeftProperty, value);
		}
	}

	public double Top
	{
		get
		{
			return GetValue(TopProperty);
		}
		set
		{
			SetValue(TopProperty, value);
		}
	}

	public double Right
	{
		get
		{
			return GetValue(RightProperty);
		}
		set
		{
			SetValue(RightProperty, value);
		}
	}

	public double Bottom
	{
		get
		{
			return GetValue(BottomProperty);
		}
		set
		{
			SetValue(BottomProperty, value);
		}
	}

	public IBrush Highlight
	{
		get
		{
			return GetValue(HighlightProperty);
		}
		set
		{
			SetValue(HighlightProperty, value);
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ThicknessProperty)
		{
			try
			{
				_isUpdatingThickness = true;
				Thickness newValue = change.GetNewValue<Thickness>();
				SetCurrentValue(LeftProperty, newValue.Left);
				SetCurrentValue(TopProperty, newValue.Top);
				SetCurrentValue(RightProperty, newValue.Right);
				SetCurrentValue(BottomProperty, newValue.Bottom);
				return;
			}
			finally
			{
				_isUpdatingThickness = false;
			}
		}
		if (!_isUpdatingThickness && (change.Property == LeftProperty || change.Property == TopProperty || change.Property == RightProperty || change.Property == BottomProperty))
		{
			SetCurrentValue(ThicknessProperty, new Thickness(Left, Top, Right, Bottom));
		}
	}
}
