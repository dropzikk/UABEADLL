using System;
using Avalonia.Collections;
using Avalonia.Rendering;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace Avalonia.Layout;

public static class LayoutHelper
{
	private readonly struct MinMax
	{
		public double MinWidth { get; }

		public double MaxWidth { get; }

		public double MinHeight { get; }

		public double MaxHeight { get; }

		public MinMax(Layoutable e)
		{
			MaxHeight = e.MaxHeight;
			MinHeight = e.MinHeight;
			double height = e.Height;
			double val = (double.IsNaN(height) ? double.PositiveInfinity : height);
			MaxHeight = Math.Max(Math.Min(val, MaxHeight), MinHeight);
			val = (double.IsNaN(height) ? 0.0 : height);
			MinHeight = Math.Max(Math.Min(MaxHeight, val), MinHeight);
			MaxWidth = e.MaxWidth;
			MinWidth = e.MinWidth;
			height = e.Width;
			double val2 = (double.IsNaN(height) ? double.PositiveInfinity : height);
			MaxWidth = Math.Max(Math.Min(val2, MaxWidth), MinWidth);
			val2 = (double.IsNaN(height) ? 0.0 : height);
			MinWidth = Math.Max(Math.Min(MaxWidth, val2), MinWidth);
		}
	}

	public static double LayoutEpsilon { get; } = 1.53E-06;

	public static Size ApplyLayoutConstraints(Layoutable control, Size constraints)
	{
		MinMax minMax = new MinMax(control);
		return new Size(MathUtilities.Clamp(constraints.Width, minMax.MinWidth, minMax.MaxWidth), MathUtilities.Clamp(constraints.Height, minMax.MinHeight, minMax.MaxHeight));
	}

	public static Size MeasureChild(Layoutable? control, Size availableSize, Thickness padding, Thickness borderThickness)
	{
		if (IsParentLayoutRounded(control, out var scale))
		{
			padding = RoundLayoutThickness(padding, scale, scale);
			borderThickness = RoundLayoutThickness(borderThickness, scale, scale);
		}
		if (control != null)
		{
			control.Measure(availableSize.Deflate(padding + borderThickness));
			return control.DesiredSize.Inflate(padding + borderThickness);
		}
		return default(Size).Inflate(padding + borderThickness);
	}

	public static Size MeasureChild(Layoutable? control, Size availableSize, Thickness padding)
	{
		if (IsParentLayoutRounded(control, out var scale))
		{
			padding = RoundLayoutThickness(padding, scale, scale);
		}
		if (control != null)
		{
			control.Measure(availableSize.Deflate(padding));
			return control.DesiredSize.Inflate(padding);
		}
		return new Size(padding.Left + padding.Right, padding.Bottom + padding.Top);
	}

	public static Size ArrangeChild(Layoutable? child, Size availableSize, Thickness padding, Thickness borderThickness)
	{
		if (IsParentLayoutRounded(child, out var scale))
		{
			padding = RoundLayoutThickness(padding, scale, scale);
			borderThickness = RoundLayoutThickness(borderThickness, scale, scale);
		}
		return ArrangeChildInternal(child, availableSize, padding + borderThickness);
	}

	public static Size ArrangeChild(Layoutable? child, Size availableSize, Thickness padding)
	{
		if (IsParentLayoutRounded(child, out var scale))
		{
			padding = RoundLayoutThickness(padding, scale, scale);
		}
		return ArrangeChildInternal(child, availableSize, padding);
	}

	private static Size ArrangeChildInternal(Layoutable? child, Size availableSize, Thickness padding)
	{
		child?.Arrange(new Rect(availableSize).Deflate(padding));
		return availableSize;
	}

	private static bool IsParentLayoutRounded(Layoutable? child, out double scale)
	{
		if (!(child?.GetVisualParent() is Layoutable { UseLayoutRounding: not false } layoutable))
		{
			scale = 1.0;
			return false;
		}
		scale = GetLayoutScale(layoutable);
		return true;
	}

	public static void InvalidateSelfAndChildrenMeasure(Layoutable control)
	{
		if (control != null)
		{
			InnerInvalidateMeasure(control);
		}
		static void InnerInvalidateMeasure(Visual target)
		{
			if (target is Layoutable layoutable)
			{
				layoutable.InvalidateMeasure();
			}
			IAvaloniaList<Visual> visualChildren = target.VisualChildren;
			int count = visualChildren.Count;
			for (int i = 0; i < count; i++)
			{
				InnerInvalidateMeasure(visualChildren[i]);
			}
		}
	}

	public static double GetLayoutScale(Layoutable control)
	{
		IRenderRoot renderRoot = control?.VisualRoot;
		double num = (renderRoot as ILayoutRoot)?.LayoutScaling ?? 1.0;
		if (num == 0.0 || double.IsNaN(num) || double.IsInfinity(num))
		{
			throw new Exception($"Invalid LayoutScaling returned from {renderRoot.GetType()}");
		}
		return num;
	}

	public static Size RoundLayoutSizeUp(Size size, double dpiScaleX, double dpiScaleY)
	{
		return new Size(RoundLayoutValueUp(size.Width, dpiScaleX), RoundLayoutValueUp(size.Height, dpiScaleY));
	}

	public static Thickness RoundLayoutThickness(Thickness thickness, double dpiScaleX, double dpiScaleY)
	{
		return new Thickness(RoundLayoutValue(thickness.Left, dpiScaleX), RoundLayoutValue(thickness.Top, dpiScaleY), RoundLayoutValue(thickness.Right, dpiScaleX), RoundLayoutValue(thickness.Bottom, dpiScaleY));
	}

	public static double RoundLayoutValue(double value, double dpiScale)
	{
		double num;
		if (!MathUtilities.IsOne(dpiScale))
		{
			num = Math.Round(value * dpiScale) / dpiScale;
			if (double.IsNaN(num) || double.IsInfinity(num) || MathUtilities.AreClose(num, double.MaxValue))
			{
				num = value;
			}
		}
		else
		{
			num = Math.Round(value);
		}
		return num;
	}

	public static double RoundLayoutValueUp(double value, double dpiScale)
	{
		value = Math.Round(value, 8, MidpointRounding.ToZero);
		double num;
		if (!MathUtilities.IsOne(dpiScale))
		{
			num = Math.Ceiling(value * dpiScale) / dpiScale;
			if (double.IsNaN(num) || double.IsInfinity(num) || MathUtilities.AreClose(num, double.MaxValue))
			{
				num = value;
			}
		}
		else
		{
			num = Math.Ceiling(value);
		}
		return num;
	}
}
