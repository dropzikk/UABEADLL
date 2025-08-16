using System;
using System.Collections.Generic;
using Avalonia.Collections.Pooled;
using Avalonia.Input.Raw;

namespace Avalonia;

internal class RawEventGrouper : IDisposable
{
	private readonly Action<RawInputEventArgs> _eventCallback;

	private readonly IRawEventGrouperDispatchQueue _queue;

	private readonly Dictionary<long, RawPointerEventArgs> _lastTouchPoints = new Dictionary<long, RawPointerEventArgs>();

	private RawInputEventArgs? _lastEvent;

	private Action<RawInputEventArgs> _dispatch;

	private bool _disposed;

	private static readonly Func<IReadOnlyList<RawPointerPoint>> s_getPooledListDelegate = GetPooledList;

	public RawEventGrouper(Action<RawInputEventArgs> eventCallback, IRawEventGrouperDispatchQueue? queue = null)
	{
		_eventCallback = eventCallback;
		_queue = queue ?? new AutomaticRawEventGrouperDispatchQueue();
		_dispatch = Dispatch;
	}

	private void AddToQueue(RawInputEventArgs args)
	{
		_lastEvent = args;
		_queue.Add(args, _dispatch);
	}

	private void Dispatch(RawInputEventArgs ev)
	{
		if (!_disposed)
		{
			if (_lastEvent == ev)
			{
				_lastEvent = null;
			}
			if (ev is RawTouchEventArgs { Type: RawPointerEventType.TouchUpdate } rawTouchEventArgs)
			{
				_lastTouchPoints.Remove(rawTouchEventArgs.RawPointerId);
			}
			_eventCallback?.Invoke(ev);
		}
		if (ev is RawPointerEventArgs rawPointerEventArgs)
		{
			Lazy<IReadOnlyList<RawPointerPoint>> intermediatePoints = rawPointerEventArgs.IntermediatePoints;
			if (intermediatePoints != null && intermediatePoints.Value is PooledList<RawPointerPoint> pooledList)
			{
				pooledList.Dispose();
			}
		}
	}

	public void HandleEvent(RawInputEventArgs args)
	{
		if (args is RawPointerEventArgs rawPointerEventArgs && _lastEvent != null && _lastEvent.Device == args.Device && _lastEvent is RawPointerEventArgs rawPointerEventArgs2 && rawPointerEventArgs2.InputModifiers == rawPointerEventArgs.InputModifiers && rawPointerEventArgs2.Type == rawPointerEventArgs.Type)
		{
			RawPointerEventType type = rawPointerEventArgs2.Type;
			if (type == RawPointerEventType.Move || type == RawPointerEventType.TouchUpdate)
			{
				if (args is RawTouchEventArgs rawTouchEventArgs)
				{
					if (_lastTouchPoints.TryGetValue(rawTouchEventArgs.RawPointerId, out RawPointerEventArgs value))
					{
						MergeEvents(value, rawTouchEventArgs);
						return;
					}
					_lastTouchPoints[rawTouchEventArgs.RawPointerId] = rawTouchEventArgs;
					AddToQueue(rawTouchEventArgs);
				}
				else
				{
					MergeEvents(rawPointerEventArgs2, rawPointerEventArgs);
				}
				return;
			}
		}
		_lastTouchPoints.Clear();
		if (args is RawTouchEventArgs { Type: RawPointerEventType.TouchUpdate } rawTouchEventArgs2)
		{
			_lastTouchPoints[rawTouchEventArgs2.RawPointerId] = rawTouchEventArgs2;
		}
		AddToQueue(args);
	}

	private static IReadOnlyList<RawPointerPoint> GetPooledList()
	{
		return new PooledList<RawPointerPoint>();
	}

	private static void MergeEvents(RawPointerEventArgs last, RawPointerEventArgs current)
	{
		if (last.IntermediatePoints == null)
		{
			Lazy<IReadOnlyList<RawPointerPoint>> lazy2 = (last.IntermediatePoints = new Lazy<IReadOnlyList<RawPointerPoint>>(s_getPooledListDelegate));
		}
		((PooledList<RawPointerPoint>)last.IntermediatePoints.Value).Add(new RawPointerPoint
		{
			Position = last.Position
		});
		last.Position = current.Position;
		last.Timestamp = current.Timestamp;
		last.InputModifiers = current.InputModifiers;
	}

	public void Dispose()
	{
		_disposed = true;
		_lastEvent = null;
		_lastTouchPoints.Clear();
	}
}
