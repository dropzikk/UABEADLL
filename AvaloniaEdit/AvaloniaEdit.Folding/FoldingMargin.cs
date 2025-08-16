using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Folding;

public class FoldingMargin : AbstractMargin
{
	internal const double SizeFactor = 1.3333333333333333;

	public static readonly AttachedProperty<IBrush> FoldingMarkerBrushProperty;

	public static readonly AttachedProperty<IBrush> FoldingMarkerBackgroundBrushProperty;

	public static readonly AttachedProperty<IBrush> SelectedFoldingMarkerBrushProperty;

	public static readonly AttachedProperty<IBrush> SelectedFoldingMarkerBackgroundBrushProperty;

	private readonly List<FoldingMarginMarker> _markers = new List<FoldingMarginMarker>();

	private Pen _foldingControlPen = new Pen(FoldingMarkerBrushProperty.GetDefaultValue(typeof(FoldingMargin)));

	private Pen _selectedFoldingControlPen = new Pen(SelectedFoldingMarkerBrushProperty.GetDefaultValue(typeof(FoldingMargin)));

	public FoldingManager FoldingManager { get; set; }

	public IBrush FoldingMarkerBrush
	{
		get
		{
			return GetValue(FoldingMarkerBrushProperty);
		}
		set
		{
			SetValue(FoldingMarkerBrushProperty, value);
		}
	}

	public IBrush FoldingMarkerBackgroundBrush
	{
		get
		{
			return GetValue(FoldingMarkerBackgroundBrushProperty);
		}
		set
		{
			SetValue(FoldingMarkerBackgroundBrushProperty, value);
		}
	}

	public IBrush SelectedFoldingMarkerBrush
	{
		get
		{
			return GetValue(SelectedFoldingMarkerBrushProperty);
		}
		set
		{
			SetValue(SelectedFoldingMarkerBrushProperty, value);
		}
	}

	public IBrush SelectedFoldingMarkerBackgroundBrush
	{
		get
		{
			return GetValue(SelectedFoldingMarkerBackgroundBrushProperty);
		}
		set
		{
			SetValue(SelectedFoldingMarkerBackgroundBrushProperty, value);
		}
	}

	static FoldingMargin()
	{
		FoldingMarkerBrushProperty = AvaloniaProperty.RegisterAttached<FoldingMargin, Control, IBrush>("FoldingMarkerBrush", Brushes.Gray, inherits: true);
		FoldingMarkerBackgroundBrushProperty = AvaloniaProperty.RegisterAttached<FoldingMargin, Control, IBrush>("FoldingMarkerBackgroundBrush", Brushes.White, inherits: true);
		SelectedFoldingMarkerBrushProperty = AvaloniaProperty.RegisterAttached<FoldingMargin, Control, IBrush>("SelectedFoldingMarkerBrush", Brushes.Black, inherits: true);
		SelectedFoldingMarkerBackgroundBrushProperty = AvaloniaProperty.RegisterAttached<FoldingMargin, Control, IBrush>("SelectedFoldingMarkerBackgroundBrush", Brushes.White, inherits: true);
		FoldingMarkerBrushProperty.Changed.Subscribe(OnUpdateBrushes);
		FoldingMarkerBackgroundBrushProperty.Changed.Subscribe(OnUpdateBrushes);
		SelectedFoldingMarkerBrushProperty.Changed.Subscribe(OnUpdateBrushes);
		SelectedFoldingMarkerBackgroundBrushProperty.Changed.Subscribe(OnUpdateBrushes);
	}

	private static void OnUpdateBrushes(AvaloniaPropertyChangedEventArgs e)
	{
		FoldingMargin foldingMargin = null;
		if (e.Sender is FoldingMargin foldingMargin2)
		{
			foldingMargin = foldingMargin2;
		}
		else if (e.Sender is TextEditor textEditor)
		{
			foldingMargin = textEditor.TextArea.LeftMargins.FirstOrDefault((Control c) => c is FoldingMargin) as FoldingMargin;
		}
		if (foldingMargin != null)
		{
			if (e.Property.Name == FoldingMarkerBrushProperty.Name)
			{
				foldingMargin._foldingControlPen = new Pen((IBrush)e.NewValue);
			}
			if (e.Property.Name == SelectedFoldingMarkerBrushProperty.Name)
			{
				foldingMargin._selectedFoldingControlPen = new Pen((IBrush)e.NewValue);
			}
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		foreach (FoldingMarginMarker marker in _markers)
		{
			marker.Measure(availableSize);
		}
		return new Size(PixelSnapHelpers.RoundToOdd(1.3333333333333333 * GetValue(TextBlock.FontSizeProperty), PixelSnapHelpers.GetPixelSize(this).Width), 0.0);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		foreach (FoldingMarginMarker marker in _markers)
		{
			int visualColumn = marker.VisualLine.GetVisualColumn(marker.FoldingSection.StartOffset - marker.VisualLine.FirstDocumentLine.Offset);
			TextLine textLine = marker.VisualLine.GetTextLine(visualColumn);
			double num = marker.VisualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextMiddle) - base.TextView.VerticalOffset;
			num -= marker.DesiredSize.Height / 2.0;
			double x = (finalSize.Width - marker.DesiredSize.Width) / 2.0;
			marker.Arrange(new Rect(PixelSnapHelpers.Round(new Point(x, num), pixelSize), marker.DesiredSize));
		}
		return finalSize;
	}

	protected override void OnTextViewVisualLinesChanged()
	{
		foreach (FoldingMarginMarker marker in _markers)
		{
			base.VisualChildren.Remove(marker);
		}
		_markers.Clear();
		InvalidateVisual();
		if (base.TextView == null || FoldingManager == null || !base.TextView.VisualLinesValid)
		{
			return;
		}
		foreach (VisualLine visualLine in base.TextView.VisualLines)
		{
			FoldingSection nextFolding = FoldingManager.GetNextFolding(visualLine.FirstDocumentLine.Offset);
			if (!(nextFolding?.StartOffset <= visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.Length))
			{
				continue;
			}
			FoldingMarginMarker foldingMarginMarker = new FoldingMarginMarker
			{
				IsExpanded = !nextFolding.IsFolded,
				VisualLine = visualLine,
				FoldingSection = nextFolding
			};
			((ISetLogicalParent)foldingMarginMarker).SetParent((ILogical?)this);
			_markers.Add(foldingMarginMarker);
			base.VisualChildren.Add(foldingMarginMarker);
			foldingMarginMarker.PropertyChanged += delegate(object o, AvaloniaPropertyChangedEventArgs args)
			{
				if (args.Property == InputElement.IsPointerOverProperty)
				{
					InvalidateVisual();
				}
			};
			InvalidateMeasure();
		}
	}

	public override void Render(DrawingContext drawingContext)
	{
		if (base.TextView != null && base.TextView.VisualLinesValid && base.TextView.VisualLines.Count != 0 && FoldingManager != null)
		{
			List<TextLine> list = base.TextView.VisualLines.SelectMany((VisualLine vl) => vl.TextLines).ToList();
			Pen[] colors = new Pen[list.Count + 1];
			Pen[] endMarker = new Pen[list.Count];
			CalculateFoldLinesForFoldingsActiveAtStart(list, colors, endMarker);
			CalculateFoldLinesForMarkers(list, colors, endMarker);
			DrawFoldLines(drawingContext, colors, endMarker);
		}
	}

	private void CalculateFoldLinesForFoldingsActiveAtStart(List<TextLine> allTextLines, Pen[] colors, Pen[] endMarker)
	{
		int offset = base.TextView.VisualLines[0].FirstDocumentLine.Offset;
		int endOffset = base.TextView.VisualLines.Last().LastDocumentLine.EndOffset;
		ReadOnlyCollection<FoldingSection> foldingsContaining = FoldingManager.GetFoldingsContaining(offset);
		int num = 0;
		foreach (FoldingSection item in foldingsContaining)
		{
			int endOffset2 = item.EndOffset;
			if (endOffset2 <= endOffset && !item.IsFolded)
			{
				int textLineIndexFromOffset = GetTextLineIndexFromOffset(allTextLines, endOffset2);
				if (textLineIndexFromOffset >= 0)
				{
					endMarker[textLineIndexFromOffset] = _foldingControlPen;
				}
			}
			if (endOffset2 > num && item.StartOffset < offset)
			{
				num = endOffset2;
			}
		}
		if (num <= 0)
		{
			return;
		}
		if (num > endOffset)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i] = _foldingControlPen;
			}
			return;
		}
		int textLineIndexFromOffset2 = GetTextLineIndexFromOffset(allTextLines, num);
		for (int j = 0; j <= textLineIndexFromOffset2; j++)
		{
			colors[j] = _foldingControlPen;
		}
	}

	private void CalculateFoldLinesForMarkers(List<TextLine> allTextLines, Pen[] colors, Pen[] endMarker)
	{
		foreach (FoldingMarginMarker marker in _markers)
		{
			int endOffset = marker.FoldingSection.EndOffset;
			int textLineIndexFromOffset = GetTextLineIndexFromOffset(allTextLines, endOffset);
			if (!marker.FoldingSection.IsFolded && textLineIndexFromOffset >= 0)
			{
				if (marker.IsPointerOver)
				{
					endMarker[textLineIndexFromOffset] = _selectedFoldingControlPen;
				}
				else if (endMarker[textLineIndexFromOffset] == null)
				{
					endMarker[textLineIndexFromOffset] = _foldingControlPen;
				}
			}
			int textLineIndexFromOffset2 = GetTextLineIndexFromOffset(allTextLines, marker.FoldingSection.StartOffset);
			if (textLineIndexFromOffset2 < 0)
			{
				continue;
			}
			for (int i = textLineIndexFromOffset2 + 1; i < colors.Length && i - 1 != textLineIndexFromOffset; i++)
			{
				if (marker.IsPointerOver)
				{
					colors[i] = _selectedFoldingControlPen;
				}
				else if (colors[i] == null)
				{
					colors[i] = _foldingControlPen;
				}
			}
		}
	}

	private void DrawFoldLines(DrawingContext drawingContext, Pen[] colors, Pen[] endMarker)
	{
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		double num = PixelSnapHelpers.PixelAlign(base.Bounds.Width / 2.0, pixelSize.Width);
		double num2 = 0.0;
		Pen pen = colors[0];
		int num3 = 0;
		foreach (VisualLine visualLine in base.TextView.VisualLines)
		{
			foreach (TextLine textLine in visualLine.TextLines)
			{
				if (endMarker[num3] != null)
				{
					double visualPos = GetVisualPos(visualLine, textLine, pixelSize.Height);
					drawingContext.DrawLine(endMarker[num3], new Point(num - pixelSize.Width / 2.0, visualPos), new Point(base.Bounds.Width, visualPos));
				}
				if (colors[num3 + 1] != pen)
				{
					double visualPos2 = GetVisualPos(visualLine, textLine, pixelSize.Height);
					if (pen != null)
					{
						drawingContext.DrawLine(pen, new Point(num, num2 + pixelSize.Height / 2.0), new Point(num, visualPos2 - pixelSize.Height / 2.0));
					}
					pen = colors[num3 + 1];
					num2 = visualPos2;
				}
				num3++;
			}
		}
		if (pen != null)
		{
			drawingContext.DrawLine(pen, new Point(num, num2 + pixelSize.Height / 2.0), new Point(num, base.Bounds.Height));
		}
	}

	private double GetVisualPos(VisualLine vl, TextLine tl, double pixelHeight)
	{
		return PixelSnapHelpers.PixelAlign(vl.GetTextLineVisualYPosition(tl, VisualYPosition.TextMiddle) - base.TextView.VerticalOffset, pixelHeight);
	}

	private int GetTextLineIndexFromOffset(List<TextLine> textLines, int offset)
	{
		int lineNumber = base.TextView.Document.GetLineByOffset(offset).LineNumber;
		VisualLine visualLine = base.TextView.GetVisualLine(lineNumber);
		if (visualLine != null)
		{
			int relativeTextOffset = offset - visualLine.FirstDocumentLine.Offset;
			TextLine textLine = visualLine.GetTextLine(visualLine.GetVisualColumn(relativeTextOffset));
			return textLines.IndexOf(textLine);
		}
		return -1;
	}
}
