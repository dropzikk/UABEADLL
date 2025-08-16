using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

internal sealed class CaretLayer : Layer
{
	private readonly TextArea _textArea;

	private bool _isVisible;

	private Rect _caretRectangle;

	private readonly DispatcherTimer _caretBlinkTimer = new DispatcherTimer();

	private bool _blink;

	internal IBrush CaretBrush;

	public CaretLayer(TextArea textArea)
		: base(textArea.TextView, KnownLayer.Caret)
	{
		_textArea = textArea;
		base.IsHitTestVisible = false;
		_caretBlinkTimer.Tick += CaretBlinkTimer_Tick;
	}

	private void CaretBlinkTimer_Tick(object sender, EventArgs e)
	{
		_blink = !_blink;
		InvalidateVisual();
	}

	public void Show(Rect caretRectangle)
	{
		_caretRectangle = caretRectangle;
		_isVisible = true;
		StartBlinkAnimation();
		InvalidateVisual();
	}

	public void Hide()
	{
		if (_isVisible)
		{
			_isVisible = false;
			StopBlinkAnimation();
			InvalidateVisual();
		}
	}

	private void StartBlinkAnimation()
	{
		TimeSpan interval = TimeSpan.FromMilliseconds(500.0);
		_blink = true;
		if (interval.TotalMilliseconds > 0.0)
		{
			_caretBlinkTimer.Interval = interval;
			_caretBlinkTimer.Start();
		}
	}

	private void StopBlinkAnimation()
	{
		_caretBlinkTimer.Stop();
	}

	public override void Render(DrawingContext drawingContext)
	{
		base.Render(drawingContext);
		if (_isVisible && _blink)
		{
			IBrush brush = CaretBrush ?? TextView.GetValue(TextBlock.ForegroundProperty);
			if (_textArea.OverstrikeMode && brush is ISolidColorBrush { Color: var color })
			{
				brush = new SolidColorBrush(Color.FromArgb(100, color.R, color.G, color.B));
			}
			Rect rect = new Rect(_caretRectangle.X - TextView.HorizontalOffset, _caretRectangle.Y - TextView.VerticalOffset, _caretRectangle.Width, _caretRectangle.Height);
			drawingContext.FillRectangle(brush, PixelSnapHelpers.Round(rect, PixelSnapHelpers.GetPixelSize(this)));
		}
	}
}
