using System;

namespace Avalonia.LogicalTree;

public class LogicalTreeAttachmentEventArgs : EventArgs
{
	public ILogicalRoot Root { get; }

	public ILogical Source { get; }

	public ILogical? Parent { get; }

	public LogicalTreeAttachmentEventArgs(ILogicalRoot root, ILogical source, ILogical? parent)
	{
		Root = root ?? throw new ArgumentNullException("root");
		Source = source ?? throw new ArgumentNullException("source");
		Parent = parent;
	}
}
