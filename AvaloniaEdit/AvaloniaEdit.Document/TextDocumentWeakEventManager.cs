using System;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public static class TextDocumentWeakEventManager
{
	public sealed class UpdateStarted : WeakEventManagerBase<UpdateStarted, TextDocument, EventHandler, EventArgs>
	{
		protected override void StartListening(TextDocument source)
		{
			source.UpdateStarted += WeakEventManagerBase<UpdateStarted, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.UpdateStarted -= WeakEventManagerBase<UpdateStarted, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}
	}

	public sealed class UpdateFinished : WeakEventManagerBase<UpdateFinished, TextDocument, EventHandler, EventArgs>
	{
		protected override void StartListening(TextDocument source)
		{
			source.UpdateFinished += WeakEventManagerBase<UpdateFinished, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.UpdateFinished -= WeakEventManagerBase<UpdateFinished, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}
	}

	public sealed class Changing : WeakEventManagerBase<Changing, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>
	{
		protected override void StartListening(TextDocument source)
		{
			source.Changing += WeakEventManagerBase<Changing, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.Changing -= WeakEventManagerBase<Changing, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.DeliverEvent;
		}
	}

	public sealed class Changed : WeakEventManagerBase<Changed, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>
	{
		protected override void StartListening(TextDocument source)
		{
			source.Changed += WeakEventManagerBase<Changed, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.Changed -= WeakEventManagerBase<Changed, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.DeliverEvent;
		}
	}

	public sealed class LineCountChanged : WeakEventManagerBase<LineCountChanged, TextDocument, EventHandler, EventArgs>
	{
		protected override void StartListening(TextDocument source)
		{
			source.LineCountChanged += WeakEventManagerBase<LineCountChanged, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.LineCountChanged -= WeakEventManagerBase<LineCountChanged, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}
	}

	public sealed class TextLengthChanged : WeakEventManagerBase<TextLengthChanged, TextDocument, EventHandler, EventArgs>
	{
		protected override void StartListening(TextDocument source)
		{
			source.TextLengthChanged += WeakEventManagerBase<TextLengthChanged, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.TextLengthChanged -= WeakEventManagerBase<TextLengthChanged, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}
	}

	public sealed class TextChanged : WeakEventManagerBase<TextChanged, TextDocument, EventHandler, EventArgs>
	{
		protected override void StartListening(TextDocument source)
		{
			source.TextChanged += WeakEventManagerBase<TextChanged, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.TextChanged -= WeakEventManagerBase<TextChanged, TextDocument, EventHandler, EventArgs>.DeliverEvent;
		}
	}
}
