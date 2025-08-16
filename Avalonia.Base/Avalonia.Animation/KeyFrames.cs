using System.Collections.Generic;
using Avalonia.Collections;

namespace Avalonia.Animation;

public sealed class KeyFrames : AvaloniaList<KeyFrame>
{
	public KeyFrames()
	{
		base.ResetBehavior = ResetBehavior.Remove;
	}

	public KeyFrames(IEnumerable<KeyFrame> items)
		: base(items)
	{
		base.ResetBehavior = ResetBehavior.Remove;
	}
}
