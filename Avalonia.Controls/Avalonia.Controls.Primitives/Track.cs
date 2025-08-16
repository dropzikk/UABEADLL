using System;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.Utilities;

namespace Avalonia.Controls.Primitives;

[PseudoClasses(new string[] { ":vertical", ":horizontal" })]
public class Track : Control
{
	public static readonly StyledProperty<double> MinimumProperty;

	public static readonly StyledProperty<double> MaximumProperty;

	public static readonly StyledProperty<double> ValueProperty;

	public static readonly StyledProperty<double> ViewportSizeProperty;

	public static readonly StyledProperty<Orientation> OrientationProperty;

	public static readonly StyledProperty<Thumb?> ThumbProperty;

	public static readonly StyledProperty<Button?> IncreaseButtonProperty;

	public static readonly StyledProperty<Button?> DecreaseButtonProperty;

	public static readonly StyledProperty<bool> IsDirectionReversedProperty;

	public static readonly StyledProperty<bool> IgnoreThumbDragProperty;

	private Vector _lastDrag;

	public double Minimum
	{
		get
		{
			return GetValue(MinimumProperty);
		}
		set
		{
			SetValue(MinimumProperty, value);
		}
	}

	public double Maximum
	{
		get
		{
			return GetValue(MaximumProperty);
		}
		set
		{
			SetValue(MaximumProperty, value);
		}
	}

	public double Value
	{
		get
		{
			return GetValue(ValueProperty);
		}
		set
		{
			SetValue(ValueProperty, value);
		}
	}

	public double ViewportSize
	{
		get
		{
			return GetValue(ViewportSizeProperty);
		}
		set
		{
			SetValue(ViewportSizeProperty, value);
		}
	}

	public Orientation Orientation
	{
		get
		{
			return GetValue(OrientationProperty);
		}
		set
		{
			SetValue(OrientationProperty, value);
		}
	}

	[Content]
	public Thumb? Thumb
	{
		get
		{
			return GetValue(ThumbProperty);
		}
		set
		{
			SetValue(ThumbProperty, value);
		}
	}

	public Button? IncreaseButton
	{
		get
		{
			return GetValue(IncreaseButtonProperty);
		}
		set
		{
			SetValue(IncreaseButtonProperty, value);
		}
	}

	public Button? DecreaseButton
	{
		get
		{
			return GetValue(DecreaseButtonProperty);
		}
		set
		{
			SetValue(DecreaseButtonProperty, value);
		}
	}

	public bool IsDirectionReversed
	{
		get
		{
			return GetValue(IsDirectionReversedProperty);
		}
		set
		{
			SetValue(IsDirectionReversedProperty, value);
		}
	}

	public bool IgnoreThumbDrag
	{
		get
		{
			return GetValue(IgnoreThumbDragProperty);
		}
		set
		{
			SetValue(IgnoreThumbDragProperty, value);
		}
	}

	private double ThumbCenterOffset { get; set; }

	private double Density { get; set; }

	static Track()
	{
		MinimumProperty = RangeBase.MinimumProperty.AddOwner<Track>();
		MaximumProperty = RangeBase.MaximumProperty.AddOwner<Track>();
		ValueProperty = RangeBase.ValueProperty.AddOwner<Track>();
		ViewportSizeProperty = ScrollBar.ViewportSizeProperty.AddOwner<Track>();
		OrientationProperty = ScrollBar.OrientationProperty.AddOwner<Track>();
		ThumbProperty = AvaloniaProperty.Register<Track, Thumb>("Thumb");
		IncreaseButtonProperty = AvaloniaProperty.Register<Track, Button>("IncreaseButton");
		DecreaseButtonProperty = AvaloniaProperty.Register<Track, Button>("DecreaseButton");
		IsDirectionReversedProperty = AvaloniaProperty.Register<Track, bool>("IsDirectionReversed", defaultValue: false);
		IgnoreThumbDragProperty = AvaloniaProperty.Register<Track, bool>("IgnoreThumbDrag", defaultValue: false);
		ThumbProperty.Changed.AddClassHandler(delegate(Track x, AvaloniaPropertyChangedEventArgs e)
		{
			x.ThumbChanged(e);
		});
		IncreaseButtonProperty.Changed.AddClassHandler(delegate(Track x, AvaloniaPropertyChangedEventArgs e)
		{
			x.ButtonChanged(e);
		});
		DecreaseButtonProperty.Changed.AddClassHandler(delegate(Track x, AvaloniaPropertyChangedEventArgs e)
		{
			x.ButtonChanged(e);
		});
		Layoutable.AffectsArrange<Track>(new AvaloniaProperty[4] { MinimumProperty, MaximumProperty, ValueProperty, OrientationProperty });
	}

	public Track()
	{
		UpdatePseudoClasses(Orientation);
	}

	public virtual double ValueFromPoint(Point point)
	{
		double val = ((Orientation != 0) ? (Value + ValueFromDistance(point.X - base.Bounds.Width * 0.5, point.Y - ThumbCenterOffset)) : (Value + ValueFromDistance(point.X - ThumbCenterOffset, point.Y - base.Bounds.Height * 0.5)));
		return Math.Max(Minimum, Math.Min(Maximum, val));
	}

	public virtual double ValueFromDistance(double horizontal, double vertical)
	{
		double num = ((!IsDirectionReversed) ? 1 : (-1));
		if (Orientation == Orientation.Horizontal)
		{
			return num * horizontal * Density;
		}
		return -1.0 * num * vertical * Density;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		Size result = new Size(0.0, 0.0);
		if (Thumb != null)
		{
			Thumb.Measure(availableSize);
			result = Thumb.DesiredSize;
		}
		if (!double.IsNaN(ViewportSize))
		{
			result = ((Orientation != Orientation.Vertical) ? result.WithWidth(0.0) : result.WithHeight(0.0));
		}
		return result;
	}

	protected override Size ArrangeOverride(Size arrangeSize)
	{
		bool flag = Orientation == Orientation.Vertical;
		double viewportSize = Math.Max(0.0, ViewportSize);
		double decreaseButtonLength;
		double thumbLength;
		double increaseButtonLength;
		if (double.IsNaN(ViewportSize))
		{
			ComputeSliderLengths(arrangeSize, flag, out decreaseButtonLength, out thumbLength, out increaseButtonLength);
		}
		else if (!ComputeScrollBarLengths(arrangeSize, viewportSize, flag, out decreaseButtonLength, out thumbLength, out increaseButtonLength))
		{
			return arrangeSize;
		}
		Point point = default(Point);
		Size size = arrangeSize;
		bool isDirectionReversed = IsDirectionReversed;
		if (flag)
		{
			CoerceLength(ref decreaseButtonLength, arrangeSize.Height);
			CoerceLength(ref increaseButtonLength, arrangeSize.Height);
			CoerceLength(ref thumbLength, arrangeSize.Height);
			point = point.WithY(isDirectionReversed ? (decreaseButtonLength + thumbLength) : 0.0);
			size = size.WithHeight(increaseButtonLength);
			if (IncreaseButton != null)
			{
				IncreaseButton.Arrange(new Rect(point, size));
			}
			point = point.WithY(isDirectionReversed ? 0.0 : (increaseButtonLength + thumbLength));
			size = size.WithHeight(decreaseButtonLength);
			if (DecreaseButton != null)
			{
				DecreaseButton.Arrange(new Rect(point, size));
			}
			point = point.WithY(isDirectionReversed ? decreaseButtonLength : increaseButtonLength);
			size = size.WithHeight(thumbLength);
			if (Thumb != null)
			{
				Rect rect = new Rect(point, size);
				Vector v = CalculateThumbAdjustment(Thumb, rect);
				Thumb.Arrange(rect);
				Thumb.AdjustDrag(v);
			}
			ThumbCenterOffset = point.Y + thumbLength * 0.5;
		}
		else
		{
			CoerceLength(ref decreaseButtonLength, arrangeSize.Width);
			CoerceLength(ref increaseButtonLength, arrangeSize.Width);
			CoerceLength(ref thumbLength, arrangeSize.Width);
			point = point.WithX(isDirectionReversed ? (increaseButtonLength + thumbLength) : 0.0);
			size = size.WithWidth(decreaseButtonLength);
			if (DecreaseButton != null)
			{
				DecreaseButton.Arrange(new Rect(point, size));
			}
			point = point.WithX(isDirectionReversed ? 0.0 : (decreaseButtonLength + thumbLength));
			size = size.WithWidth(increaseButtonLength);
			if (IncreaseButton != null)
			{
				IncreaseButton.Arrange(new Rect(point, size));
			}
			point = point.WithX(isDirectionReversed ? increaseButtonLength : decreaseButtonLength);
			size = size.WithWidth(thumbLength);
			if (Thumb != null)
			{
				Rect rect2 = new Rect(point, size);
				Vector v2 = CalculateThumbAdjustment(Thumb, rect2);
				Thumb.Arrange(rect2);
				Thumb.AdjustDrag(v2);
			}
			ThumbCenterOffset = point.X + thumbLength * 0.5;
		}
		_lastDrag = default(Vector);
		return arrangeSize;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == OrientationProperty)
		{
			UpdatePseudoClasses(change.GetNewValue<Orientation>());
		}
	}

	private Vector CalculateThumbAdjustment(Thumb thumb, Rect newThumbBounds)
	{
		Point point = newThumbBounds.Position - thumb.Bounds.Position;
		return _lastDrag - point;
	}

	private static void CoerceLength(ref double componentLength, double trackLength)
	{
		if (componentLength < 0.0)
		{
			componentLength = 0.0;
		}
		else if (componentLength > trackLength || double.IsNaN(componentLength))
		{
			componentLength = trackLength;
		}
	}

	private void ComputeSliderLengths(Size arrangeSize, bool isVertical, out double decreaseButtonLength, out double thumbLength, out double increaseButtonLength)
	{
		double minimum = Minimum;
		double num = Math.Max(0.0, Maximum - minimum);
		double num2 = Math.Min(num, Value - minimum);
		double num3;
		if (isVertical)
		{
			num3 = arrangeSize.Height;
			thumbLength = ((Thumb == null) ? 0.0 : Thumb.DesiredSize.Height);
		}
		else
		{
			num3 = arrangeSize.Width;
			thumbLength = ((Thumb == null) ? 0.0 : Thumb.DesiredSize.Width);
		}
		CoerceLength(ref thumbLength, num3);
		double num4 = num3 - thumbLength;
		decreaseButtonLength = num4 * num2 / num;
		CoerceLength(ref decreaseButtonLength, num4);
		increaseButtonLength = num4 - decreaseButtonLength;
		CoerceLength(ref increaseButtonLength, num4);
		Density = num / num4;
	}

	private bool ComputeScrollBarLengths(Size arrangeSize, double viewportSize, bool isVertical, out double decreaseButtonLength, out double thumbLength, out double increaseButtonLength)
	{
		double minimum = Minimum;
		double num = Math.Max(0.0, Maximum - minimum);
		double num2 = Math.Min(num, Value - minimum);
		double num3 = Math.Max(0.0, num) + viewportSize;
		double num4 = (isVertical ? arrangeSize.Height : arrangeSize.Width);
		double val = 10.0;
		StyledProperty<double> property = (isVertical ? Layoutable.MinHeightProperty : Layoutable.MinWidthProperty);
		Thumb thumb = Thumb;
		if (thumb != null && thumb.IsSet(property))
		{
			val = thumb.GetValue(property);
		}
		thumbLength = num4 * viewportSize / num3;
		CoerceLength(ref thumbLength, num4);
		thumbLength = Math.Max(val, thumbLength);
		bool num5 = MathUtilities.LessThanOrClose(num, 0.0);
		bool flag = thumbLength > num4;
		if (num5 || flag)
		{
			ShowChildren(visible: false);
			ThumbCenterOffset = double.NaN;
			Density = double.NaN;
			decreaseButtonLength = 0.0;
			increaseButtonLength = 0.0;
			return false;
		}
		ShowChildren(visible: true);
		double num6 = num4 - thumbLength;
		decreaseButtonLength = num6 * num2 / num;
		CoerceLength(ref decreaseButtonLength, num6);
		increaseButtonLength = num6 - decreaseButtonLength;
		CoerceLength(ref increaseButtonLength, num6);
		Density = num / num6;
		return true;
	}

	private void ThumbChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Thumb thumb = (Thumb)e.OldValue;
		Thumb thumb2 = (Thumb)e.NewValue;
		if (thumb != null)
		{
			thumb.DragDelta -= ThumbDragged;
			base.LogicalChildren.Remove(thumb);
			base.VisualChildren.Remove(thumb);
		}
		if (thumb2 != null)
		{
			thumb2.DragDelta += ThumbDragged;
			base.LogicalChildren.Add(thumb2);
			base.VisualChildren.Add(thumb2);
		}
	}

	private void ButtonChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Button button = (Button)e.OldValue;
		Button button2 = (Button)e.NewValue;
		if (button != null)
		{
			base.LogicalChildren.Remove(button);
			base.VisualChildren.Remove(button);
		}
		if (button2 != null)
		{
			base.LogicalChildren.Add(button2);
			base.VisualChildren.Add(button2);
		}
	}

	private void ThumbDragged(object? sender, VectorEventArgs e)
	{
		if (!IgnoreThumbDrag)
		{
			double value = Value;
			double num = ValueFromDistance(e.Vector.X, e.Vector.Y);
			Vector vector = e.Vector / num;
			SetCurrentValue(ValueProperty, MathUtilities.Clamp(value + num, Minimum, Maximum));
			_lastDrag = (Value - value) * vector;
		}
	}

	private void ShowChildren(bool visible)
	{
		if (Thumb != null)
		{
			Thumb.IsVisible = visible;
		}
		if (IncreaseButton != null)
		{
			IncreaseButton.IsVisible = visible;
		}
		if (DecreaseButton != null)
		{
			DecreaseButton.IsVisible = visible;
		}
	}

	private void UpdatePseudoClasses(Orientation o)
	{
		base.PseudoClasses.Set(":vertical", o == Orientation.Vertical);
		base.PseudoClasses.Set(":horizontal", o == Orientation.Horizontal);
	}
}
