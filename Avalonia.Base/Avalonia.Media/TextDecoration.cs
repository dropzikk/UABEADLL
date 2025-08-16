using System.Collections.Generic;
using Avalonia.Collections;
using Avalonia.Media.TextFormatting;

namespace Avalonia.Media;

public class TextDecoration : AvaloniaObject
{
	public static readonly StyledProperty<TextDecorationLocation> LocationProperty = AvaloniaProperty.Register<TextDecoration, TextDecorationLocation>("Location", TextDecorationLocation.Underline);

	public static readonly StyledProperty<IBrush?> StrokeProperty = AvaloniaProperty.Register<TextDecoration, IBrush>("Stroke");

	public static readonly StyledProperty<TextDecorationUnit> StrokeThicknessUnitProperty = AvaloniaProperty.Register<TextDecoration, TextDecorationUnit>("StrokeThicknessUnit", TextDecorationUnit.FontRecommended);

	public static readonly StyledProperty<AvaloniaList<double>?> StrokeDashArrayProperty = AvaloniaProperty.Register<TextDecoration, AvaloniaList<double>>("StrokeDashArray");

	public static readonly StyledProperty<double> StrokeDashOffsetProperty = AvaloniaProperty.Register<TextDecoration, double>("StrokeDashOffset", 0.0);

	public static readonly StyledProperty<double> StrokeThicknessProperty = AvaloniaProperty.Register<TextDecoration, double>("StrokeThickness", 1.0);

	public static readonly StyledProperty<PenLineCap> StrokeLineCapProperty = AvaloniaProperty.Register<TextDecoration, PenLineCap>("StrokeLineCap", PenLineCap.Flat);

	public static readonly StyledProperty<double> StrokeOffsetProperty = AvaloniaProperty.Register<TextDecoration, double>("StrokeOffset", 0.0);

	public static readonly StyledProperty<TextDecorationUnit> StrokeOffsetUnitProperty = AvaloniaProperty.Register<TextDecoration, TextDecorationUnit>("StrokeOffsetUnit", TextDecorationUnit.FontRecommended);

	public TextDecorationLocation Location
	{
		get
		{
			return GetValue(LocationProperty);
		}
		set
		{
			SetValue(LocationProperty, value);
		}
	}

	public IBrush? Stroke
	{
		get
		{
			return GetValue(StrokeProperty);
		}
		set
		{
			SetValue(StrokeProperty, value);
		}
	}

	public TextDecorationUnit StrokeThicknessUnit
	{
		get
		{
			return GetValue(StrokeThicknessUnitProperty);
		}
		set
		{
			SetValue(StrokeThicknessUnitProperty, value);
		}
	}

	public AvaloniaList<double>? StrokeDashArray
	{
		get
		{
			return GetValue(StrokeDashArrayProperty);
		}
		set
		{
			SetValue(StrokeDashArrayProperty, value);
		}
	}

	public double StrokeDashOffset
	{
		get
		{
			return GetValue(StrokeDashOffsetProperty);
		}
		set
		{
			SetValue(StrokeDashOffsetProperty, value);
		}
	}

	public double StrokeThickness
	{
		get
		{
			return GetValue(StrokeThicknessProperty);
		}
		set
		{
			SetValue(StrokeThicknessProperty, value);
		}
	}

	public PenLineCap StrokeLineCap
	{
		get
		{
			return GetValue(StrokeLineCapProperty);
		}
		set
		{
			SetValue(StrokeLineCapProperty, value);
		}
	}

	public double StrokeOffset
	{
		get
		{
			return GetValue(StrokeOffsetProperty);
		}
		set
		{
			SetValue(StrokeOffsetProperty, value);
		}
	}

	public TextDecorationUnit StrokeOffsetUnit
	{
		get
		{
			return GetValue(StrokeOffsetUnitProperty);
		}
		set
		{
			SetValue(StrokeOffsetUnitProperty, value);
		}
	}

	internal void Draw(DrawingContext drawingContext, GlyphRun glyphRun, TextMetrics textMetrics, IBrush defaultBrush)
	{
		Point baselineOrigin = glyphRun.BaselineOrigin;
		double num = StrokeThickness;
		switch (StrokeThicknessUnit)
		{
		case TextDecorationUnit.FontRecommended:
			switch (Location)
			{
			case TextDecorationLocation.Underline:
				num = textMetrics.UnderlineThickness;
				break;
			case TextDecorationLocation.Strikethrough:
				num = textMetrics.StrikethroughThickness;
				break;
			}
			break;
		case TextDecorationUnit.FontRenderingEmSize:
			num = textMetrics.FontRenderingEmSize * num;
			break;
		}
		Point point = default(Point);
		switch (Location)
		{
		case TextDecorationLocation.Baseline:
			point += glyphRun.BaselineOrigin;
			break;
		case TextDecorationLocation.Strikethrough:
			point += new Point(baselineOrigin.X, baselineOrigin.Y + textMetrics.StrikethroughPosition);
			break;
		case TextDecorationLocation.Underline:
			point += new Point(baselineOrigin.X, baselineOrigin.Y + textMetrics.UnderlinePosition);
			break;
		}
		switch (StrokeOffsetUnit)
		{
		case TextDecorationUnit.FontRenderingEmSize:
			point += new Point(0.0, StrokeOffset * textMetrics.FontRenderingEmSize);
			break;
		case TextDecorationUnit.Pixel:
			point += new Point(0.0, StrokeOffset);
			break;
		}
		Pen pen = new Pen(Stroke ?? defaultBrush, num, new DashStyle(StrokeDashArray, StrokeDashOffset), StrokeLineCap);
		if (Location != TextDecorationLocation.Strikethrough)
		{
			double num2 = glyphRun.BaselineOrigin.Y - point.Y;
			IReadOnlyList<float> intersections = glyphRun.GetIntersections((float)(num * 0.5 - num2), (float)(num * 1.5 - num2));
			if (intersections.Count > 0)
			{
				double num3 = baselineOrigin.X;
				double num4 = num3 + glyphRun.Bounds.Width;
				double num5 = num3;
				List<double> list = new List<double>();
				for (int i = 0; i < intersections.Count; i += 2)
				{
					double num6 = (double)intersections[i] - num;
					num5 = (double)intersections[i + 1] + num;
					if (num6 > num3 && num3 + textMetrics.FontRenderingEmSize / 12.0 < num6)
					{
						list.Add(num3);
						list.Add(num6);
					}
					num3 = num5;
				}
				if (num5 < num4)
				{
					list.Add(num5);
					list.Add(num4);
				}
				for (int j = 0; j < list.Count; j += 2)
				{
					Point p = new Point(list[j], point.Y);
					Point p2 = new Point(list[j + 1], point.Y);
					drawingContext.DrawLine(pen, p, p2);
				}
				return;
			}
		}
		drawingContext.DrawLine(pen, point, point + new Point(glyphRun.Metrics.Width, 0.0));
	}
}
