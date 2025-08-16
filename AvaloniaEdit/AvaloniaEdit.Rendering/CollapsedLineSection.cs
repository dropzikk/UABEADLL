using AvaloniaEdit.Document;

namespace AvaloniaEdit.Rendering;

public sealed class CollapsedLineSection
{
	private readonly HeightTree _heightTree;

	private const string Id = "";

	public bool IsCollapsed => Start != null;

	public DocumentLine Start { get; internal set; }

	public DocumentLine End { get; internal set; }

	internal CollapsedLineSection(HeightTree heightTree, DocumentLine start, DocumentLine end)
	{
		_heightTree = heightTree;
		Start = start;
		End = end;
	}

	public void Uncollapse()
	{
		if (Start != null)
		{
			if (!_heightTree.IsDisposed)
			{
				_heightTree.Uncollapse(this);
			}
			Start = null;
			End = null;
		}
	}

	public override string ToString()
	{
		return "[CollapsedSection Start=" + ((Start != null) ? Start.LineNumber.ToString() : "null") + " End=" + ((End != null) ? End.LineNumber.ToString() : "null") + "]";
	}
}
