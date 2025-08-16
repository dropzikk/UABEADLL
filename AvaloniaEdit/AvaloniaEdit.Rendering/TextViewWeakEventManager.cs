using System;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

public static class TextViewWeakEventManager
{
	public sealed class DocumentChanged : WeakEventManagerBase<DocumentChanged, TextView, EventHandler, EventArgs>
	{
		protected override void StartListening(TextView source)
		{
			source.DocumentChanged += WeakEventManagerBase<DocumentChanged, TextView, EventHandler, EventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextView source)
		{
			source.DocumentChanged -= WeakEventManagerBase<DocumentChanged, TextView, EventHandler, EventArgs>.DeliverEvent;
		}
	}

	public sealed class VisualLinesChanged : WeakEventManagerBase<VisualLinesChanged, TextView, EventHandler, EventArgs>
	{
		protected override void StartListening(TextView source)
		{
			source.VisualLinesChanged += WeakEventManagerBase<VisualLinesChanged, TextView, EventHandler, EventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextView source)
		{
			source.VisualLinesChanged -= WeakEventManagerBase<VisualLinesChanged, TextView, EventHandler, EventArgs>.DeliverEvent;
		}
	}

	public sealed class ScrollOffsetChanged : WeakEventManagerBase<ScrollOffsetChanged, TextView, EventHandler, EventArgs>
	{
		protected override void StartListening(TextView source)
		{
			source.ScrollOffsetChanged += WeakEventManagerBase<ScrollOffsetChanged, TextView, EventHandler, EventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextView source)
		{
			source.ScrollOffsetChanged -= WeakEventManagerBase<ScrollOffsetChanged, TextView, EventHandler, EventArgs>.DeliverEvent;
		}
	}
}
