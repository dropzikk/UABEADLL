using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace AvaloniaEdit.Search;

internal class SearchResultBackgroundRenderer : IBackgroundRenderer
{
	private IBrush _markerBrush;

	public TextSegmentCollection<SearchResult> CurrentResults { get; } = new TextSegmentCollection<SearchResult>();

	public KnownLayer Layer => KnownLayer.Background;

	public IBrush MarkerBrush
	{
		get
		{
			return _markerBrush;
		}
		set
		{
			_markerBrush = value;
		}
	}

	public SearchResultBackgroundRenderer(IBrush brush)
	{
		_markerBrush = brush;
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (textView == null)
		{
			throw new ArgumentNullException("textView");
		}
		if (drawingContext == null)
		{
			throw new ArgumentNullException("drawingContext");
		}
		if (CurrentResults == null || !textView.VisualLinesValid)
		{
			return;
		}
		ReadOnlyCollection<VisualLine> visualLines = textView.VisualLines;
		if (visualLines.Count == 0)
		{
			return;
		}
		int offset = visualLines.First().FirstDocumentLine.Offset;
		int endOffset = visualLines.Last().LastDocumentLine.EndOffset;
		foreach (SearchResult item in CurrentResults.FindOverlappingSegments(offset, endOffset - offset))
		{
			BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
			backgroundGeometryBuilder.AlignToWholePixels = true;
			backgroundGeometryBuilder.CornerRadius = 0.0;
			backgroundGeometryBuilder.AddSegment(textView, item);
			Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
			if (geometry != null)
			{
				drawingContext.DrawGeometry(_markerBrush, null, geometry);
			}
		}
	}
}
