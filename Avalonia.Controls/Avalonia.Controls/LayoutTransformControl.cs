using System;
using Avalonia.Media;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class LayoutTransformControl : Decorator
{
	public static readonly StyledProperty<ITransform?> LayoutTransformProperty;

	public static readonly StyledProperty<bool> UseRenderTransformProperty;

	private IDisposable? _renderTransformChangedEvent;

	private const double AcceptableDelta = 0.0001;

	private const int DecimalsAfterRound = 4;

	private Size _childActualSize;

	private readonly MatrixTransform _matrixTransform = new MatrixTransform();

	private Matrix _transformation = Matrix.Identity;

	private IDisposable? _transformChangedEvent;

	public ITransform? LayoutTransform
	{
		get
		{
			return GetValue(LayoutTransformProperty);
		}
		set
		{
			SetValue(LayoutTransformProperty, value);
		}
	}

	public bool UseRenderTransform
	{
		get
		{
			return GetValue(UseRenderTransformProperty);
		}
		set
		{
			SetValue(UseRenderTransformProperty, value);
		}
	}

	public Control? TransformRoot => base.Child;

	static LayoutTransformControl()
	{
		LayoutTransformProperty = AvaloniaProperty.Register<LayoutTransformControl, ITransform>("LayoutTransform");
		UseRenderTransformProperty = AvaloniaProperty.Register<LayoutTransformControl, bool>("UseRenderTransform", defaultValue: false);
		Visual.ClipToBoundsProperty.OverrideDefaultValue<LayoutTransformControl>(defaultValue: true);
		LayoutTransformProperty.Changed.AddClassHandler(delegate(LayoutTransformControl x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnLayoutTransformChanged(e);
		});
		Decorator.ChildProperty.Changed.AddClassHandler(delegate(LayoutTransformControl x, AvaloniaPropertyChangedEventArgs _)
		{
			x.OnChildChanged();
		});
		UseRenderTransformProperty.Changed.AddClassHandler(delegate(LayoutTransformControl x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnUseRenderTransformPropertyChanged(e);
		});
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (TransformRoot == null || LayoutTransform == null)
		{
			SetCurrentValue(LayoutTransformProperty, base.RenderTransform);
			return base.ArrangeOverride(finalSize);
		}
		Size a = ComputeLargestTransformedSize(finalSize);
		if (IsSizeSmaller(a, TransformRoot.DesiredSize))
		{
			a = TransformRoot.DesiredSize;
		}
		Rect rect = new Rect(0.0, 0.0, a.Width, a.Height).TransformToAABB(_transformation);
		Rect rect2 = new Rect(0.0 - rect.X + (finalSize.Width - rect.Width) / 2.0, 0.0 - rect.Y + (finalSize.Height - rect.Height) / 2.0, a.Width, a.Height);
		TransformRoot.Arrange(rect2);
		Size size = TransformRoot.Bounds.Size;
		if (!IsSizeSmaller(a, size) || !(_childActualSize == default(Size)))
		{
			_childActualSize = default(Size);
		}
		return finalSize;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (TransformRoot == null || LayoutTransform == null)
		{
			return base.MeasureOverride(availableSize);
		}
		Size availableSize2 = ((!(_childActualSize == default(Size))) ? _childActualSize : ComputeLargestTransformedSize(availableSize));
		TransformRoot.Measure(availableSize2);
		Size desiredSize = TransformRoot.DesiredSize;
		Rect rect = new Rect(0.0, 0.0, desiredSize.Width, desiredSize.Height).TransformToAABB(_transformation);
		return new Size(rect.Width, rect.Height);
	}

	private void OnUseRenderTransformPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		LayoutTransformControl obj = e.Sender as LayoutTransformControl;
		bool flag = (bool)e.NewValue;
		if (obj == null)
		{
			return;
		}
		if (flag)
		{
			_renderTransformChangedEvent = Visual.RenderTransformProperty.Changed.Subscribe(delegate(AvaloniaPropertyChangedEventArgs<ITransform> x)
			{
				if (x.Sender is LayoutTransformControl layoutTransformControl)
				{
					layoutTransformControl.LayoutTransform = layoutTransformControl.RenderTransform;
				}
			});
		}
		else
		{
			_renderTransformChangedEvent?.Dispose();
			ClearValue(LayoutTransformProperty);
		}
	}

	private void OnChildChanged()
	{
		if (TransformRoot != null)
		{
			TransformRoot.RenderTransform = _matrixTransform;
			TransformRoot.RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Absolute);
		}
		ApplyLayoutTransform();
	}

	private static bool IsSizeSmaller(Size a, Size b)
	{
		if (!(a.Width + 0.0001 < b.Width))
		{
			return a.Height + 0.0001 < b.Height;
		}
		return true;
	}

	private static Matrix RoundMatrix(Matrix matrix, int decimals)
	{
		return new Matrix(Math.Round(matrix.M11, decimals), Math.Round(matrix.M12, decimals), Math.Round(matrix.M21, decimals), Math.Round(matrix.M22, decimals), matrix.M31, matrix.M32);
	}

	private void ApplyLayoutTransform()
	{
		Matrix matrix = ((LayoutTransform == null) ? Matrix.Identity : RoundMatrix(LayoutTransform.Value, 4));
		if (!(_transformation == matrix))
		{
			_transformation = matrix;
			_matrixTransform.Matrix = matrix;
			InvalidateMeasure();
		}
	}

	private Size ComputeLargestTransformedSize(Size arrangeBounds)
	{
		Size result = default(Size);
		bool flag = double.IsInfinity(arrangeBounds.Width);
		if (flag)
		{
			arrangeBounds = arrangeBounds.WithWidth(arrangeBounds.Height);
		}
		bool flag2 = double.IsInfinity(arrangeBounds.Height);
		if (flag2)
		{
			arrangeBounds = arrangeBounds.WithHeight(arrangeBounds.Width);
		}
		double m = _transformation.M11;
		double m2 = _transformation.M12;
		double m3 = _transformation.M21;
		double m4 = _transformation.M22;
		double num = Math.Abs(arrangeBounds.Width / m);
		double num2 = Math.Abs(arrangeBounds.Width / m3);
		double num3 = Math.Abs(arrangeBounds.Height / m2);
		double num4 = Math.Abs(arrangeBounds.Height / m4);
		double num5 = num / 2.0;
		double num6 = num2 / 2.0;
		double num7 = num3 / 2.0;
		double num8 = num4 / 2.0;
		double num9 = 0.0 - num2 / num;
		double num10 = 0.0 - num4 / num3;
		if (0.0 == arrangeBounds.Width || 0.0 == arrangeBounds.Height)
		{
			result = new Size(arrangeBounds.Width, arrangeBounds.Height);
		}
		else if (flag && flag2)
		{
			result = new Size(double.PositiveInfinity, double.PositiveInfinity);
		}
		else if (!_transformation.HasInverse)
		{
			result = new Size(0.0, 0.0);
		}
		else if (0.0 == m2 || 0.0 == m3)
		{
			double num11 = (flag2 ? double.PositiveInfinity : num4);
			double num12 = (flag ? double.PositiveInfinity : num);
			if (0.0 == m2 && 0.0 == m3)
			{
				result = new Size(num12, num11);
			}
			else if (0.0 == m2)
			{
				double num13 = Math.Min(num6, num11);
				result = new Size(num12 - Math.Abs(m3 * num13 / m), num13);
			}
			else if (0.0 == m3)
			{
				double num14 = Math.Min(num7, num12);
				result = new Size(num14, num11 - Math.Abs(m2 * num14 / m4));
			}
		}
		else if (0.0 == m || 0.0 == m4)
		{
			double num15 = (flag2 ? double.PositiveInfinity : num3);
			double num16 = (flag ? double.PositiveInfinity : num2);
			if (0.0 == m && 0.0 == m4)
			{
				result = new Size(num15, num16);
			}
			else if (0.0 == m)
			{
				double num17 = Math.Min(num8, num16);
				result = new Size(num15 - Math.Abs(m4 * num17 / m2), num17);
			}
			else if (0.0 == m4)
			{
				double num18 = Math.Min(num5, num15);
				result = new Size(num18, num16 - Math.Abs(m * num18 / m3));
			}
		}
		else if (num6 <= num10 * num5 + num4)
		{
			result = new Size(num5, num6);
		}
		else if (num8 <= num9 * num7 + num2)
		{
			result = new Size(num7, num8);
		}
		else
		{
			double num19 = (num4 - num2) / (num9 - num10);
			result = new Size(num19, num9 * num19 + num2);
		}
		return result;
	}

	private void OnLayoutTransformChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Transform newTransform = e.NewValue as Transform;
		_transformChangedEvent?.Dispose();
		_transformChangedEvent = null;
		if (newTransform != null)
		{
			_transformChangedEvent = Observable.FromEventPattern(delegate(EventHandler v)
			{
				newTransform.Changed += v;
			}, delegate(EventHandler v)
			{
				newTransform.Changed -= v;
			}).Subscribe(delegate
			{
				ApplyLayoutTransform();
			});
		}
		ApplyLayoutTransform();
	}
}
