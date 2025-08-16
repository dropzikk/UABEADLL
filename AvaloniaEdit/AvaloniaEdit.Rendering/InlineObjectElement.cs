using System;
using Avalonia.Controls;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

public class InlineObjectElement : VisualLineElement
{
	public Control Element { get; }

	public InlineObjectElement(int documentLength, Control element)
		: base(1, documentLength)
	{
		Element = element ?? throw new ArgumentNullException("element");
	}

	public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		return new InlineObjectRun(1, base.TextRunProperties, Element);
	}
}
