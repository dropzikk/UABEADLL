using System;

namespace AvaloniaEdit.Snippets;

public class SnippetEventArgs : EventArgs
{
	public DeactivateReason Reason { get; }

	public SnippetEventArgs(DeactivateReason reason)
	{
		Reason = reason;
	}
}
