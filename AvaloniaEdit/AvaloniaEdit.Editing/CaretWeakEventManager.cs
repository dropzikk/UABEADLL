using System;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

public static class CaretWeakEventManager
{
	public sealed class PositionChanged : WeakEventManagerBase<PositionChanged, Caret, EventHandler, EventArgs>
	{
		protected override void StartListening(Caret source)
		{
			source.PositionChanged += WeakEventManagerBase<PositionChanged, Caret, EventHandler, EventArgs>.DeliverEvent;
		}

		protected override void StopListening(Caret source)
		{
			source.PositionChanged -= WeakEventManagerBase<PositionChanged, Caret, EventHandler, EventArgs>.DeliverEvent;
		}
	}
}
