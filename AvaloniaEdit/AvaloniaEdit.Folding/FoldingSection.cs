using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Folding;

public sealed class FoldingSection : TextSegment
{
	private readonly FoldingManager _manager;

	private bool _isFolded;

	private string _title;

	internal CollapsedLineSection[] CollapsedSections;

	public bool IsFolded
	{
		get
		{
			return _isFolded;
		}
		set
		{
			if (_isFolded != value)
			{
				_isFolded = value;
				ValidateCollapsedLineSections();
				_manager.Redraw(this);
			}
		}
	}

	public string Title
	{
		get
		{
			return _title;
		}
		set
		{
			if (_title != value)
			{
				_title = value;
				if (IsFolded)
				{
					_manager.Redraw(this);
				}
			}
		}
	}

	public string TextContent => _manager.Document.GetText(base.StartOffset, base.EndOffset - base.StartOffset);

	public object Tag { get; set; }

	internal void ValidateCollapsedLineSections()
	{
		if (!_isFolded)
		{
			RemoveCollapsedLineSection();
			return;
		}
		DocumentLine lineByOffset = _manager.Document.GetLineByOffset(base.StartOffset.CoerceValue(0, _manager.Document.TextLength));
		DocumentLine lineByOffset2 = _manager.Document.GetLineByOffset(base.EndOffset.CoerceValue(0, _manager.Document.TextLength));
		if (lineByOffset == lineByOffset2)
		{
			RemoveCollapsedLineSection();
			return;
		}
		if (CollapsedSections == null)
		{
			CollapsedSections = new CollapsedLineSection[_manager.TextViews.Count];
		}
		DocumentLine nextLine = lineByOffset.NextLine;
		for (int i = 0; i < CollapsedSections.Length; i++)
		{
			CollapsedLineSection collapsedLineSection = CollapsedSections[i];
			if (collapsedLineSection == null || collapsedLineSection.Start != nextLine || collapsedLineSection.End != lineByOffset2)
			{
				collapsedLineSection?.Uncollapse();
				CollapsedSections[i] = _manager.TextViews[i].CollapseLines(nextLine, lineByOffset2);
			}
		}
	}

	protected override void OnSegmentChanged()
	{
		ValidateCollapsedLineSections();
		base.OnSegmentChanged();
		if (base.IsConnectedToCollection)
		{
			_manager.Redraw(this);
		}
	}

	internal FoldingSection(FoldingManager manager, int startOffset, int endOffset)
	{
		_manager = manager;
		base.StartOffset = startOffset;
		base.Length = endOffset - startOffset;
	}

	private void RemoveCollapsedLineSection()
	{
		if (CollapsedSections == null)
		{
			return;
		}
		CollapsedLineSection[] collapsedSections = CollapsedSections;
		foreach (CollapsedLineSection collapsedLineSection in collapsedSections)
		{
			if (collapsedLineSection?.Start != null)
			{
				collapsedLineSection.Uncollapse();
			}
		}
		CollapsedSections = null;
	}
}
