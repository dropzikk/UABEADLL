using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

internal sealed class ColumnRulerRenderer : IBackgroundRenderer
{
	private IPen _pen;

	private IEnumerable<int> _columns;

	private readonly TextView _textView;

	public static readonly Color DefaultForeground = Colors.LightGray;

	public KnownLayer Layer => KnownLayer.Background;

	public ColumnRulerRenderer(TextView textView)
	{
		_pen = new ImmutablePen(new ImmutableSolidColorBrush(DefaultForeground));
		_textView = textView ?? throw new ArgumentNullException("textView");
		_textView.BackgroundRenderers.Add(this);
	}

	public void SetRuler(IEnumerable<int> columns, IPen pen)
	{
		_columns = columns;
		_pen = pen;
		_textView.InvalidateLayer(Layer);
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (_columns == null)
		{
			return;
		}
		foreach (int column in _columns)
		{
			double num = PixelSnapHelpers.PixelAlign(textView.WideSpaceWidth * (double)column, PixelSnapHelpers.GetPixelSize(textView).Width);
			num -= textView.ScrollOffset.X;
			Point p = new Point(num, 0.0);
			Point p2 = new Point(num, Math.Max(textView.DocumentHeight, textView.Bounds.Height));
			drawingContext.DrawLine(_pen, p.SnapToDevicePixels(textView), p2.SnapToDevicePixels(textView));
		}
	}
}
