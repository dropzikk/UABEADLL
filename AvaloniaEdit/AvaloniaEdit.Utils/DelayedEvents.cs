using System;
using System.Collections.Generic;

namespace AvaloniaEdit.Utils;

internal sealed class DelayedEvents
{
	private struct EventCall
	{
		private readonly EventHandler _handler;

		private readonly object _sender;

		private readonly EventArgs _e;

		public EventCall(EventHandler handler, object sender, EventArgs e)
		{
			_handler = handler;
			_sender = sender;
			_e = e;
		}

		public void Call()
		{
			_handler(_sender, _e);
		}
	}

	private readonly Queue<EventCall> _eventCalls = new Queue<EventCall>();

	public void DelayedRaise(EventHandler handler, object sender, EventArgs e)
	{
		if (handler != null)
		{
			_eventCalls.Enqueue(new EventCall(handler, sender, e));
		}
	}

	public void RaiseEvents()
	{
		while (_eventCalls.Count > 0)
		{
			_eventCalls.Dequeue().Call();
		}
	}
}
