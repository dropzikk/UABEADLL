using System;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Controls.Utils;

internal class BorderRenderHelper
{
	private class BorderGeometryKeypoints
	{
		internal Point LeftTop { get; }

		internal Point TopLeft { get; }

		internal Point TopRight { get; }

		internal Point RightTop { get; }

		internal Point RightBottom { get; }

		internal Point BottomRight { get; }

		internal Point BottomLeft { get; }

		internal Point LeftBottom { get; }

		internal BorderGeometryKeypoints(Rect boundRect, Thickness borderThickness, CornerRadius cornerRadius, bool inner)
		{
			double num = 0.5 * borderThickness.Left;
			double num2 = 0.5 * borderThickness.Top;
			double num3 = 0.5 * borderThickness.Right;
			double num4 = 0.5 * borderThickness.Bottom;
			double num5;
			double num6;
			double num7;
			double num8;
			double num9;
			double num10;
			double num11;
			double num12;
			if (inner)
			{
				num5 = Math.Max(0.0, cornerRadius.TopLeft - num2) + boundRect.TopLeft.Y;
				num6 = Math.Max(0.0, cornerRadius.TopLeft - num) + boundRect.TopLeft.X;
				num7 = boundRect.Width - Math.Max(0.0, cornerRadius.TopRight - num2) + boundRect.TopLeft.X;
				num8 = Math.Max(0.0, cornerRadius.TopRight - num3) + boundRect.TopLeft.Y;
				num9 = boundRect.Height - Math.Max(0.0, cornerRadius.BottomRight - num4) + boundRect.TopLeft.Y;
				num10 = boundRect.Width - Math.Max(0.0, cornerRadius.BottomRight - num3) + boundRect.TopLeft.X;
				num11 = Math.Max(0.0, cornerRadius.BottomLeft - num) + boundRect.TopLeft.X;
				num12 = boundRect.Height - Math.Max(0.0, cornerRadius.BottomLeft - num4) + boundRect.TopLeft.Y;
			}
			else
			{
				num5 = cornerRadius.TopLeft + num2 + boundRect.TopLeft.Y;
				num6 = cornerRadius.TopLeft + num + boundRect.TopLeft.X;
				num7 = boundRect.Width - (cornerRadius.TopRight + num3) + boundRect.TopLeft.X;
				num8 = cornerRadius.TopRight + num2 + boundRect.TopLeft.Y;
				num9 = boundRect.Height - (cornerRadius.BottomRight + num4) + boundRect.TopLeft.Y;
				num10 = boundRect.Width - (cornerRadius.BottomRight + num3) + boundRect.TopLeft.X;
				num11 = cornerRadius.BottomLeft + num + boundRect.TopLeft.X;
				num12 = boundRect.Height - (cornerRadius.BottomLeft + num4) + boundRect.TopLeft.Y;
			}
			double x = boundRect.TopLeft.X;
			double y = boundRect.TopLeft.Y;
			double y2 = boundRect.TopLeft.Y;
			double x2 = boundRect.Width + boundRect.TopLeft.X;
			double x3 = boundRect.Width + boundRect.TopLeft.X;
			double y3 = boundRect.Height + boundRect.TopLeft.Y;
			double y4 = boundRect.Height + boundRect.TopLeft.Y;
			double x4 = boundRect.TopLeft.X;
			LeftTop = new Point(x, num5);
			TopLeft = new Point(num6, y);
			TopRight = new Point(num7, y2);
			RightTop = new Point(x2, num8);
			RightBottom = new Point(x3, num9);
			BottomRight = new Point(num10, y3);
			BottomLeft = new Point(num11, y4);
			LeftBottom = new Point(x4, num12);
			if (TopLeft.X > TopRight.X)
			{
				double x5 = num6 / (num6 + num7) * boundRect.Width;
				TopLeft = new Point(x5, TopLeft.Y);
				TopRight = new Point(x5, TopRight.Y);
			}
			if (RightTop.Y > RightBottom.Y)
			{
				double y5 = num9 / (num8 + num9) * boundRect.Height;
				RightTop = new Point(RightTop.X, y5);
				RightBottom = new Point(RightBottom.X, y5);
			}
			if (BottomRight.X < BottomLeft.X)
			{
				double x6 = num11 / (num11 + num10) * boundRect.Width;
				BottomRight = new Point(x6, BottomRight.Y);
				BottomLeft = new Point(x6, BottomLeft.Y);
			}
			if (LeftBottom.Y < LeftTop.Y)
			{
				double y6 = num5 / (num5 + num12) * boundRect.Height;
				LeftBottom = new Point(LeftBottom.X, y6);
				LeftTop = new Point(LeftTop.X, y6);
			}
		}
	}

	private bool _useComplexRendering;

	private bool? _backendSupportsIndividualCorners;

	private StreamGeometry? _backgroundGeometryCache;

	private StreamGeometry? _borderGeometryCache;

	private Size _size;

	private Thickness _borderThickness;

	private CornerRadius _cornerRadius;

	private bool _initialized;

	private void Update(Size finalSize, Thickness borderThickness, CornerRadius cornerRadius)
	{
		bool valueOrDefault = _backendSupportsIndividualCorners == true;
		if (!_backendSupportsIndividualCorners.HasValue)
		{
			valueOrDefault = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>().SupportsIndividualRoundRects;
			_backendSupportsIndividualCorners = valueOrDefault;
		}
		_size = finalSize;
		_borderThickness = borderThickness;
		_cornerRadius = cornerRadius;
		_initialized = true;
		if (borderThickness.IsUniform && (cornerRadius.IsUniform || _backendSupportsIndividualCorners == true))
		{
			_backgroundGeometryCache = null;
			_borderGeometryCache = null;
			_useComplexRendering = false;
			return;
		}
		_useComplexRendering = true;
		Rect boundRect = new Rect(finalSize);
		Rect boundRect2 = boundRect.Deflate(borderThickness);
		BorderGeometryKeypoints keypoints = null;
		StreamGeometry streamGeometry = null;
		if (boundRect2.Width != 0.0 && boundRect2.Height != 0.0)
		{
			streamGeometry = new StreamGeometry();
			keypoints = new BorderGeometryKeypoints(boundRect2, borderThickness, cornerRadius, inner: true);
			using (StreamGeometryContext context = streamGeometry.Open())
			{
				CreateGeometry(context, boundRect2, keypoints);
			}
			_backgroundGeometryCache = streamGeometry;
		}
		else
		{
			_backgroundGeometryCache = null;
		}
		if (boundRect.Width != 0.0 && boundRect.Height != 0.0)
		{
			BorderGeometryKeypoints keypoints2 = new BorderGeometryKeypoints(boundRect, borderThickness, cornerRadius, inner: false);
			StreamGeometry streamGeometry2 = new StreamGeometry();
			using (StreamGeometryContext context2 = streamGeometry2.Open())
			{
				CreateGeometry(context2, boundRect, keypoints2);
				if (streamGeometry != null)
				{
					CreateGeometry(context2, boundRect2, keypoints);
				}
			}
			_borderGeometryCache = streamGeometry2;
		}
		else
		{
			_borderGeometryCache = null;
		}
	}

	public void Render(DrawingContext context, Size finalSize, Thickness borderThickness, CornerRadius cornerRadius, IBrush? background, IBrush? borderBrush, BoxShadows boxShadows, double borderDashOffset = 0.0, PenLineCap borderLineCap = PenLineCap.Flat, PenLineJoin borderLineJoin = PenLineJoin.Miter, AvaloniaList<double>? borderDashArray = null)
	{
		if (_size != finalSize || _borderThickness != borderThickness || _cornerRadius != cornerRadius || !_initialized)
		{
			Update(finalSize, borderThickness, cornerRadius);
		}
		RenderCore(context, background, borderBrush, boxShadows, borderDashOffset, borderLineCap, borderLineJoin, borderDashArray);
	}

	private void RenderCore(DrawingContext context, IBrush? background, IBrush? borderBrush, BoxShadows boxShadows, double borderDashOffset, PenLineCap borderLineCap, PenLineJoin borderLineJoin, AvaloniaList<double>? borderDashArray)
	{
		if (_useComplexRendering)
		{
			StreamGeometry backgroundGeometryCache = _backgroundGeometryCache;
			if (backgroundGeometryCache != null)
			{
				context.DrawGeometry(background, null, backgroundGeometryCache);
			}
			StreamGeometry borderGeometryCache = _borderGeometryCache;
			if (borderGeometryCache != null)
			{
				context.DrawGeometry(borderBrush, null, borderGeometryCache);
			}
			return;
		}
		double top = _borderThickness.Top;
		IPen pen = null;
		ImmutableDashStyle dashStyle = null;
		if (borderDashArray != null && borderDashArray.Count > 0)
		{
			dashStyle = new ImmutableDashStyle(borderDashArray, borderDashOffset);
		}
		if (borderBrush != null && top > 0.0)
		{
			pen = new ImmutablePen(borderBrush.ToImmutable(), top, dashStyle, borderLineCap, borderLineJoin);
		}
		Rect rect = new Rect(_size);
		if (!MathUtilities.IsZero(top))
		{
			rect = rect.Deflate(top * 0.5);
		}
		RoundedRect rrect = new RoundedRect(rect, _cornerRadius.TopLeft, _cornerRadius.TopRight, _cornerRadius.BottomRight, _cornerRadius.BottomLeft);
		context.DrawRectangle(background, pen, rrect, boxShadows);
	}

	private static void CreateGeometry(StreamGeometryContext context, Rect boundRect, BorderGeometryKeypoints keypoints)
	{
		context.BeginFigure(keypoints.TopLeft, isFilled: true);
		context.LineTo(keypoints.TopRight);
		double num = boundRect.TopRight.X - keypoints.TopRight.X;
		double num2 = keypoints.RightTop.Y - boundRect.TopRight.Y;
		if (num != 0.0 || num2 != 0.0)
		{
			context.ArcTo(keypoints.RightTop, new Size(num, num2), 0.0, isLargeArc: false, SweepDirection.Clockwise);
		}
		context.LineTo(keypoints.RightBottom);
		num = boundRect.BottomRight.X - keypoints.BottomRight.X;
		num2 = boundRect.BottomRight.Y - keypoints.RightBottom.Y;
		if (num != 0.0 || num2 != 0.0)
		{
			context.ArcTo(keypoints.BottomRight, new Size(num, num2), 0.0, isLargeArc: false, SweepDirection.Clockwise);
		}
		context.LineTo(keypoints.BottomLeft);
		num = keypoints.BottomLeft.X - boundRect.BottomLeft.X;
		num2 = boundRect.BottomLeft.Y - keypoints.LeftBottom.Y;
		if (num != 0.0 || num2 != 0.0)
		{
			context.ArcTo(keypoints.LeftBottom, new Size(num, num2), 0.0, isLargeArc: false, SweepDirection.Clockwise);
		}
		context.LineTo(keypoints.LeftTop);
		num = keypoints.TopLeft.X - boundRect.TopLeft.X;
		num2 = keypoints.LeftTop.Y - boundRect.TopLeft.Y;
		if (num != 0.0 || num2 != 0.0)
		{
			context.ArcTo(keypoints.TopLeft, new Size(num, num2), 0.0, isLargeArc: false, SweepDirection.Clockwise);
		}
		context.EndFigure(isClosed: true);
	}
}
