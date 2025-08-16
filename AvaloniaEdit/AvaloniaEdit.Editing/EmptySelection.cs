using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

internal sealed class EmptySelection : Selection
{
	public override TextViewPosition StartPosition => new TextViewPosition(TextLocation.Empty);

	public override TextViewPosition EndPosition => new TextViewPosition(TextLocation.Empty);

	public override ISegment SurroundingSegment => null;

	public override IEnumerable<SelectionSegment> Segments => Empty<SelectionSegment>.Array;

	public override int Length => 0;

	public EmptySelection(TextArea textArea)
		: base(textArea)
	{
	}

	public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
	{
		return this;
	}

	public override Selection SetEndpoint(TextViewPosition endPosition)
	{
		throw new NotSupportedException();
	}

	public override Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition)
	{
		if (base.TextArea.Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return Selection.Create(base.TextArea, startPosition, endPosition);
	}

	public override string GetText()
	{
		return string.Empty;
	}

	public override void ReplaceSelectionWithText(string newText)
	{
		if (newText == null)
		{
			throw new ArgumentNullException("newText");
		}
		newText = AddSpacesIfRequired(newText, base.TextArea.Caret.Position, base.TextArea.Caret.Position);
		if (newText.Length > 0 && base.TextArea.ReadOnlySectionProvider.CanInsert(base.TextArea.Caret.Offset))
		{
			base.TextArea.Document.Insert(base.TextArea.Caret.Offset, newText);
		}
		base.TextArea.Caret.VisualColumn = -1;
	}

	public override int GetHashCode()
	{
		return RuntimeHelpers.GetHashCode(this);
	}

	public override bool Equals(object obj)
	{
		return this == obj;
	}
}
