using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AvaloniaEdit.Rendering;

internal sealed class CurrentLineHighlightRenderer : IBackgroundRenderer
{
	private int _line;

	private readonly TextView _textView;

	public static readonly Color DefaultBackground = Color.FromArgb(22, 20, 220, 224);

	public static readonly Color DefaultBorder = Color.FromArgb(52, 0, byte.MaxValue, 110);

	public int Line
	{
		get
		{
			return _line;
		}
		set
		{
			if (_line != value)
			{
				_line = value;
				_textView.InvalidateLayer(Layer);
			}
		}
	}

	public KnownLayer Layer => KnownLayer.Background;

	public IBrush BackgroundBrush { get; set; }

	public IPen BorderPen { get; set; }

	public CurrentLineHighlightRenderer(TextView textView)
	{
		BorderPen = new ImmutablePen(new ImmutableSolidColorBrush(DefaultBorder));
		BackgroundBrush = new ImmutableSolidColorBrush(DefaultBackground);
		_textView = textView ?? throw new ArgumentNullException("textView");
		_textView.BackgroundRenderers.Add(this);
		_line = 0;
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (!_textView.Options.HighlightCurrentLine)
		{
			return;
		}
		BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
		VisualLine visualLine = _textView.GetVisualLine(_line);
		if (visualLine != null)
		{
			double y = visualLine.VisualTop - _textView.ScrollOffset.Y;
			backgroundGeometryBuilder.AddRectangle(textView, new Rect(0.0, y, textView.Bounds.Width, visualLine.Height));
			Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
			if (geometry != null)
			{
				drawingContext.DrawGeometry(BackgroundBrush, BorderPen, geometry);
			}
		}
	}
}
