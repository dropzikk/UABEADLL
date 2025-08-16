using System;
using Avalonia.Rendering;

namespace Avalonia;

public class VisualTreeAttachmentEventArgs : EventArgs
{
	public Visual Parent { get; }

	public IRenderRoot Root { get; }

	public VisualTreeAttachmentEventArgs(Visual parent, IRenderRoot root)
	{
		Parent = parent ?? throw new ArgumentNullException("parent");
		Root = root ?? throw new ArgumentNullException("root");
	}
}
