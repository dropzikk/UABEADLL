using System;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Rendering;

public class VisualLineConstructionStartEventArgs : EventArgs
{
	public DocumentLine FirstLineInView { get; }

	public VisualLineConstructionStartEventArgs(DocumentLine firstLineInView)
	{
		FirstLineInView = firstLineInView ?? throw new ArgumentNullException("firstLineInView");
	}
}
