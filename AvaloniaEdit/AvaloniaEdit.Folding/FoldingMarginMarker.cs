using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Folding;

internal sealed class FoldingMarginMarker : Control
{
	internal VisualLine VisualLine;

	internal FoldingSection FoldingSection;

	private bool _isExpanded;

	private const double MarginSizeFactor = 0.7;

	public bool IsExpanded
	{
		get
		{
			return _isExpanded;
		}
		set
		{
			if (_isExpanded != value)
			{
				_isExpanded = value;
				InvalidateVisual();
			}
			if (FoldingSection != null)
			{
				FoldingSection.IsFolded = !value;
			}
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (!e.Handled)
		{
			IsExpanded = !IsExpanded;
			e.Handled = true;
		}
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		base.OnPointerMoved(e);
		base.Cursor = Avalonia.Input.Cursor.Default;
	}

	protected override Size MeasureCore(Size availableSize)
	{
		double num = PixelSnapHelpers.RoundToOdd(0.9333333333333332 * GetValue(TextBlock.FontSizeProperty), PixelSnapHelpers.GetPixelSize(this).Width);
		return new Size(num, num);
	}

	public override void Render(DrawingContext drawingContext)
	{
		FoldingMargin foldingMargin = (FoldingMargin)base.Parent;
		Pen pen = new Pen(foldingMargin.SelectedFoldingMarkerBrush, 1.0, null, PenLineCap.Square);
		Pen pen2 = new Pen(foldingMargin.FoldingMarkerBrush, 1.0, null, PenLineCap.Square);
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		Rect rect = new Rect(pixelSize.Width / 2.0, pixelSize.Height / 2.0, base.Bounds.Width - pixelSize.Width, base.Bounds.Height - pixelSize.Height);
		drawingContext.FillRectangle(base.IsPointerOver ? foldingMargin.SelectedFoldingMarkerBackgroundBrush : foldingMargin.FoldingMarkerBackgroundBrush, rect);
		drawingContext.DrawRectangle(base.IsPointerOver ? pen : pen2, rect);
		double x = rect.X + rect.Width / 2.0;
		double y = rect.Y + rect.Height / 2.0;
		double num = PixelSnapHelpers.Round(rect.Width / 8.0, pixelSize.Width) + pixelSize.Width;
		drawingContext.DrawLine(pen, new Point(rect.X + num, y), new Point(rect.Right - num, y));
		if (!_isExpanded)
		{
			drawingContext.DrawLine(pen, new Point(x, rect.Y + num), new Point(x, rect.Bottom - num));
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == InputElement.IsPointerOverProperty)
		{
			InvalidateVisual();
		}
	}
}
