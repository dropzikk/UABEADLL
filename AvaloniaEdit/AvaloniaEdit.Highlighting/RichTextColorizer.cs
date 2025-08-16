using System;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace AvaloniaEdit.Highlighting;

public class RichTextColorizer : DocumentColorizingTransformer
{
	private readonly RichTextModel _richTextModel;

	public RichTextColorizer(RichTextModel richTextModel)
	{
		_richTextModel = richTextModel ?? throw new ArgumentNullException("richTextModel");
	}

	protected override void ColorizeLine(DocumentLine line)
	{
		foreach (HighlightedSection section in _richTextModel.GetHighlightedSections(line.Offset, line.Length))
		{
			if (!HighlightingColorizer.IsEmptyColor(section.Color))
			{
				ChangeLinePart(section.Offset, section.Offset + section.Length, delegate(VisualLineElement visualLineElement)
				{
					HighlightingColorizer.ApplyColorToElement(visualLineElement, section.Color, base.CurrentContext);
				});
			}
		}
	}
}
