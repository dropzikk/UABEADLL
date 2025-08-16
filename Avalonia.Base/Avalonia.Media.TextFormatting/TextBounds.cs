using System.Collections.Generic;
using System.Diagnostics;

namespace Avalonia.Media.TextFormatting;

public sealed class TextBounds
{
	public Rect Rectangle { get; internal set; }

	public FlowDirection FlowDirection { get; }

	public IList<TextRunBounds> TextRunBounds { get; }

	[DebuggerStepThrough]
	internal TextBounds(Rect bounds, FlowDirection flowDirection, IList<TextRunBounds> runBounds)
	{
		Rectangle = bounds;
		FlowDirection = flowDirection;
		TextRunBounds = runBounds;
	}
}
