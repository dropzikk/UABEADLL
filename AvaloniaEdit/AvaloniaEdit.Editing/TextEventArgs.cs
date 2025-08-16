using System;

namespace AvaloniaEdit.Editing;

public class TextEventArgs : EventArgs
{
	public string Text { get; }

	public TextEventArgs(string text)
	{
		Text = text ?? throw new ArgumentNullException("text");
	}
}
