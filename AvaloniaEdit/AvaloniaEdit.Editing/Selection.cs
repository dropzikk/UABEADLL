using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

public abstract class Selection
{
	internal TextArea TextArea { get; }

	public abstract TextViewPosition StartPosition { get; }

	public abstract TextViewPosition EndPosition { get; }

	public abstract IEnumerable<SelectionSegment> Segments { get; }

	public abstract ISegment SurroundingSegment { get; }

	public virtual bool IsEmpty => Length == 0;

	public virtual bool EnableVirtualSpace => TextArea.Options.EnableVirtualSpace;

	public abstract int Length { get; }

	public virtual bool IsMultiline
	{
		get
		{
			ISegment surroundingSegment = SurroundingSegment;
			if (surroundingSegment == null)
			{
				return false;
			}
			int offset = surroundingSegment.Offset;
			int offset2 = offset + surroundingSegment.Length;
			TextDocument document = TextArea.Document;
			if (document == null)
			{
				throw ThrowUtil.NoDocumentAssigned();
			}
			return document.GetLineByOffset(offset) != document.GetLineByOffset(offset2);
		}
	}

	public static Selection Create(TextArea textArea, int startOffset, int endOffset)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		if (startOffset == endOffset)
		{
			return textArea.EmptySelection;
		}
		return new SimpleSelection(textArea, new TextViewPosition(textArea.Document.GetLocation(startOffset)), new TextViewPosition(textArea.Document.GetLocation(endOffset)));
	}

	internal static Selection Create(TextArea textArea, TextViewPosition start, TextViewPosition end)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		if (textArea.Document.GetOffset(start.Location) == textArea.Document.GetOffset(end.Location) && start.VisualColumn == end.VisualColumn)
		{
			return textArea.EmptySelection;
		}
		return new SimpleSelection(textArea, start, end);
	}

	public static Selection Create(TextArea textArea, ISegment segment)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		return Create(textArea, segment.Offset, segment.EndOffset);
	}

	protected Selection(TextArea textArea)
	{
		TextArea = textArea ?? throw new ArgumentNullException("textArea");
	}

	public abstract void ReplaceSelectionWithText(string newText);

	internal string AddSpacesIfRequired(string newText, TextViewPosition start, TextViewPosition end)
	{
		if (EnableVirtualSpace && InsertVirtualSpaces(newText, start, end))
		{
			DocumentLine lineByNumber = TextArea.Document.GetLineByNumber(start.Line);
			string text = TextArea.Document.GetText(lineByNumber);
			VisualLine orConstructVisualLine = TextArea.TextView.GetOrConstructVisualLine(lineByNumber);
			int num = start.VisualColumn - orConstructVisualLine.VisualLengthWithEndOfLineMarker;
			if (num > 0)
			{
				string text2 = "";
				if (!TextArea.Options.ConvertTabsToSpaces && text.Trim(new char[1] { '\t' }).Length == 0)
				{
					int num2 = num / TextArea.Options.IndentationSize;
					text2 = new string('\t', num2);
					num -= num2 * TextArea.Options.IndentationSize;
				}
				text2 += new string(' ', num);
				return text2 + newText;
			}
		}
		return newText;
	}

	private bool InsertVirtualSpaces(string newText, TextViewPosition start, TextViewPosition end)
	{
		if ((!string.IsNullOrEmpty(newText) || !IsInVirtualSpace(start) || !IsInVirtualSpace(end)) && newText != "\r\n" && newText != "\n")
		{
			return newText != "\r";
		}
		return false;
	}

	private bool IsInVirtualSpace(TextViewPosition pos)
	{
		return pos.VisualColumn > TextArea.TextView.GetOrConstructVisualLine(TextArea.Document.GetLineByNumber(pos.Line)).VisualLength;
	}

	public abstract Selection UpdateOnDocumentChange(DocumentChangeEventArgs e);

	public abstract Selection SetEndpoint(TextViewPosition endPosition);

	public abstract Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition);

	public virtual string GetText()
	{
		TextDocument document = TextArea.Document;
		if (document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		StringBuilder stringBuilder = null;
		string text = null;
		foreach (SelectionSegment segment in Segments)
		{
			if (text != null)
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(text);
				}
				else
				{
					stringBuilder.Append(text);
				}
			}
			text = document.GetText(segment);
		}
		if (stringBuilder != null)
		{
			if (text != null)
			{
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}
		return text ?? string.Empty;
	}

	public abstract override bool Equals(object obj);

	public abstract override int GetHashCode();

	public virtual bool Contains(int offset)
	{
		if (IsEmpty)
		{
			return false;
		}
		if (SurroundingSegment.Contains(offset, 0))
		{
			return Segments.Any((SelectionSegment s) => s.Contains(offset, 0));
		}
		return false;
	}
}
