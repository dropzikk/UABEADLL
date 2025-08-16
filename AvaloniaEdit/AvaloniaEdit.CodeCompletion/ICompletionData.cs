using System;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;

namespace AvaloniaEdit.CodeCompletion;

public interface ICompletionData
{
	IImage Image { get; }

	string Text { get; }

	object Content { get; }

	object Description { get; }

	double Priority { get; }

	void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs);
}
