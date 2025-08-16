using System;
using Avalonia.Metadata;

namespace Avalonia.Rendering;

[PrivateApi]
public class SceneInvalidatedEventArgs : EventArgs
{
	public Rect DirtyRect { get; }

	public IRenderRoot RenderRoot { get; }

	public SceneInvalidatedEventArgs(IRenderRoot root, Rect dirtyRect)
	{
		RenderRoot = root;
		DirtyRect = dirtyRect;
	}
}
