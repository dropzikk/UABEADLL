using System;
using System.Diagnostics;
using System.Globalization;

namespace AvaloniaEdit.Document;

public sealed class DocumentLine : IDocumentLine, ISegment
{
	private bool _isDeleted;

	private int _totalLength;

	private byte _delimiterLength;

	public bool IsDeleted
	{
		get
		{
			return _isDeleted;
		}
		internal set
		{
			_isDeleted = value;
		}
	}

	public int LineNumber
	{
		get
		{
			if (IsDeleted)
			{
				throw new InvalidOperationException();
			}
			return DocumentLineTree.GetIndexFromNode(this) + 1;
		}
	}

	public int Offset
	{
		get
		{
			if (IsDeleted)
			{
				throw new InvalidOperationException();
			}
			return DocumentLineTree.GetOffsetFromNode(this);
		}
	}

	public int EndOffset => Offset + Length;

	public int Length => _totalLength - _delimiterLength;

	public int TotalLength
	{
		get
		{
			return _totalLength;
		}
		internal set
		{
			_totalLength = value;
		}
	}

	public int DelimiterLength
	{
		get
		{
			return _delimiterLength;
		}
		internal set
		{
			_delimiterLength = (byte)value;
		}
	}

	public DocumentLine NextLine
	{
		get
		{
			if (Right != null)
			{
				return Right.LeftMost;
			}
			DocumentLine documentLine = this;
			DocumentLine documentLine2;
			do
			{
				documentLine2 = documentLine;
				documentLine = documentLine.Parent;
			}
			while (documentLine != null && documentLine.Right == documentLine2);
			return documentLine;
		}
	}

	public DocumentLine PreviousLine
	{
		get
		{
			if (Left != null)
			{
				return Left.RightMost;
			}
			DocumentLine documentLine = this;
			DocumentLine documentLine2;
			do
			{
				documentLine2 = documentLine;
				documentLine = documentLine.Parent;
			}
			while (documentLine != null && documentLine.Left == documentLine2);
			return documentLine;
		}
	}

	IDocumentLine IDocumentLine.NextLine => NextLine;

	IDocumentLine IDocumentLine.PreviousLine => PreviousLine;

	internal DocumentLine Left { get; set; }

	internal DocumentLine Right { get; set; }

	internal DocumentLine Parent { get; set; }

	internal bool Color { get; set; }

	internal DocumentLine LeftMost
	{
		get
		{
			DocumentLine documentLine = this;
			while (documentLine.Left != null)
			{
				documentLine = documentLine.Left;
			}
			return documentLine;
		}
	}

	internal DocumentLine RightMost
	{
		get
		{
			DocumentLine documentLine = this;
			while (documentLine.Right != null)
			{
				documentLine = documentLine.Right;
			}
			return documentLine;
		}
	}

	internal int NodeTotalCount { get; set; }

	internal int NodeTotalLength { get; set; }

	internal DocumentLine(TextDocument document)
	{
	}

	[Conditional("DEBUG")]
	private void DebugVerifyAccess()
	{
	}

	public override string ToString()
	{
		if (IsDeleted)
		{
			return "[DocumentLine deleted]";
		}
		return string.Format(CultureInfo.InvariantCulture, "[DocumentLine Number={0} Offset={1} Length={2}]", LineNumber, Offset, Length);
	}

	internal void ResetLine()
	{
		_totalLength = (_delimiterLength = 0);
		bool isDeleted = (Color = false);
		_isDeleted = isDeleted;
		DocumentLine documentLine2 = (Parent = null);
		DocumentLine left = (Right = documentLine2);
		Left = left;
	}

	internal DocumentLine InitLineNode()
	{
		NodeTotalCount = 1;
		NodeTotalLength = TotalLength;
		return this;
	}
}
