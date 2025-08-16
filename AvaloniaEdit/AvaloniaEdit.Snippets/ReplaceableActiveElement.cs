using System;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Snippets;

internal sealed class ReplaceableActiveElement : IReplaceableActiveElement, IActiveElement
{
	private sealed class Renderer : IBackgroundRenderer
	{
		private static readonly IBrush BackgroundBrush = CreateBackgroundBrush();

		private static readonly Pen ActiveBorderPen = CreateBorderPen();

		internal ReplaceableActiveElement Element;

		public KnownLayer Layer { get; set; }

		private static IBrush CreateBackgroundBrush()
		{
			return new ImmutableSolidColorBrush(Colors.LimeGreen, 0.4);
		}

		private static Pen CreateBorderPen()
		{
			return new Pen(Brushes.Black, 1.0, DashStyle.Dot);
		}

		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			ISegment segment = Element.Segment;
			if (segment == null)
			{
				return;
			}
			BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder
			{
				AlignToWholePixels = true,
				BorderThickness = (ActiveBorderPen?.Thickness ?? 0.0)
			};
			if (Layer == KnownLayer.Background)
			{
				backgroundGeometryBuilder.AddSegment(textView, segment);
				drawingContext.DrawGeometry(BackgroundBrush, null, backgroundGeometryBuilder.CreateGeometry());
			}
			else
			{
				if (!Element._isCaretInside)
				{
					return;
				}
				backgroundGeometryBuilder.AddSegment(textView, segment);
				foreach (BoundActiveElement item in Element._context.ActiveElements.OfType<BoundActiveElement>())
				{
					if (item.TargetElement == Element)
					{
						backgroundGeometryBuilder.AddSegment(textView, item.Segment);
						backgroundGeometryBuilder.CloseFigure();
					}
				}
				drawingContext.DrawGeometry(null, ActiveBorderPen, backgroundGeometryBuilder.CreateGeometry());
			}
		}
	}

	private readonly InsertionContext _context;

	private readonly int _startOffset;

	private readonly int _endOffset;

	private TextAnchor _start;

	private TextAnchor _end;

	private bool _isCaretInside;

	private Renderer _background;

	private Renderer _foreground;

	public string Text { get; private set; }

	public bool IsEditable => true;

	public ISegment Segment
	{
		get
		{
			if (_start.IsDeleted || _end.IsDeleted)
			{
				return null;
			}
			return new SimpleSegment(_start.Offset, Math.Max(0, _end.Offset - _start.Offset));
		}
	}

	public event EventHandler TextChanged;

	public ReplaceableActiveElement(InsertionContext context, int startOffset, int endOffset)
	{
		_context = context;
		_startOffset = startOffset;
		_endOffset = endOffset;
	}

	private void AnchorDeleted(object sender, EventArgs e)
	{
		_context.Deactivate(new SnippetEventArgs(DeactivateReason.Deleted));
	}

	public void OnInsertionCompleted()
	{
		_start = _context.Document.CreateAnchor(_startOffset);
		_start.MovementType = AnchorMovementType.BeforeInsertion;
		_end = _context.Document.CreateAnchor(_endOffset);
		_end.MovementType = AnchorMovementType.AfterInsertion;
		_start.Deleted += AnchorDeleted;
		_end.Deleted += AnchorDeleted;
		WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument, EventHandler, EventArgs>.AddHandler(_context.Document, OnDocumentTextChanged);
		_background = new Renderer
		{
			Layer = KnownLayer.Background,
			Element = this
		};
		_foreground = new Renderer
		{
			Layer = KnownLayer.Text,
			Element = this
		};
		_context.TextArea.TextView.BackgroundRenderers.Add(_background);
		_context.TextArea.TextView.BackgroundRenderers.Add(_foreground);
		_context.TextArea.Caret.PositionChanged += Caret_PositionChanged;
		Caret_PositionChanged(null, null);
		Text = GetText();
	}

	public void Deactivate(SnippetEventArgs e)
	{
		WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument, EventHandler, EventArgs>.RemoveHandler(_context.Document, OnDocumentTextChanged);
		_context.TextArea.TextView.BackgroundRenderers.Remove(_background);
		_context.TextArea.TextView.BackgroundRenderers.Remove(_foreground);
		_context.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
	}

	private void Caret_PositionChanged(object sender, EventArgs e)
	{
		ISegment segment = Segment;
		if (segment != null)
		{
			bool flag = segment.Contains(_context.TextArea.Caret.Offset, 0);
			if (flag != _isCaretInside)
			{
				_isCaretInside = flag;
				_context.TextArea.TextView.InvalidateLayer(_foreground.Layer);
			}
		}
	}

	private string GetText()
	{
		if (_start.IsDeleted || _end.IsDeleted)
		{
			return string.Empty;
		}
		return _context.Document.GetText(_start.Offset, Math.Max(0, _end.Offset - _start.Offset));
	}

	private void OnDocumentTextChanged(object sender, EventArgs e)
	{
		string text = GetText();
		if (Text != text)
		{
			Text = text;
			this.TextChanged?.Invoke(this, e);
		}
	}
}
