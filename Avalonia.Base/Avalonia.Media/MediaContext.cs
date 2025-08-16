using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.Animation;
using Avalonia.Layout;
using Avalonia.Platform;
using Avalonia.Reactive;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Threading;

namespace Avalonia.Media;

internal class MediaContext : ICompositorScheduler
{
	private class MediaContextClock : IGlobalClock, IClock, IObservable<TimeSpan>
	{
		private readonly MediaContext _parent;

		private List<IObserver<TimeSpan>> _observers = new List<IObserver<TimeSpan>>();

		private List<IObserver<TimeSpan>> _newObservers = new List<IObserver<TimeSpan>>();

		private Queue<Action<TimeSpan>> _queuedAnimationFrames = new Queue<Action<TimeSpan>>();

		private Queue<Action<TimeSpan>> _queuedAnimationFramesNext = new Queue<Action<TimeSpan>>();

		private TimeSpan _currentAnimationTimestamp;

		public bool HasNewSubscriptions => _newObservers.Count > 0;

		public bool HasSubscriptions
		{
			get
			{
				if (_observers.Count <= 0)
				{
					return _queuedAnimationFrames.Count > 0;
				}
				return true;
			}
		}

		public PlayState PlayState
		{
			get
			{
				return PlayState.Run;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public MediaContextClock(MediaContext parent)
		{
			_parent = parent;
		}

		public IDisposable Subscribe(IObserver<TimeSpan> observer)
		{
			_parent.ScheduleRender(now: false);
			Dispatcher.UIThread.VerifyAccess();
			_observers.Add(observer);
			_newObservers.Add(observer);
			return Disposable.Create(delegate
			{
				Dispatcher.UIThread.VerifyAccess();
				_observers.Remove(observer);
			});
		}

		public void RequestAnimationFrame(Action<TimeSpan> action)
		{
			_parent.ScheduleRender(now: false);
			_queuedAnimationFrames.Enqueue(action);
		}

		public void Pulse(TimeSpan now)
		{
			_newObservers.Clear();
			_currentAnimationTimestamp = now;
			Queue<Action<TimeSpan>> queuedAnimationFramesNext = _queuedAnimationFramesNext;
			Queue<Action<TimeSpan>> queuedAnimationFrames = _queuedAnimationFrames;
			_queuedAnimationFrames = queuedAnimationFramesNext;
			_queuedAnimationFramesNext = queuedAnimationFrames;
			Queue<Action<TimeSpan>> queuedAnimationFramesNext2 = _queuedAnimationFramesNext;
			Action<TimeSpan> result;
			while (queuedAnimationFramesNext2.TryDequeue(out result))
			{
				result(now);
			}
			IObserver<TimeSpan>[] array = _observers.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnNext(_currentAnimationTimestamp);
			}
		}

		public void PulseNewSubscriptions()
		{
			IObserver<TimeSpan>[] array = _newObservers.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnNext(_currentAnimationTimestamp);
			}
			_newObservers.Clear();
		}
	}

	private record TopLevelInfo(Compositor Compositor, CompositingRenderer Renderer, ILayoutManager LayoutManager);

	private readonly MediaContextClock _clock;

	private readonly Stopwatch _time = Stopwatch.StartNew();

	private bool _scheduleCommitOnLastCompositionBatchCompletion;

	private DispatcherOperation? _nextRenderOp;

	private DispatcherOperation? _inputMarkerOp;

	private TimeSpan _inputMarkerAddedAt;

	private bool _isRendering;

	private bool _animationsAreWaitingForComposition;

	private const double MaxSecondsWithoutInput = 1.0;

	private readonly Action _render;

	private readonly Action _inputMarkerHandler;

	private readonly HashSet<Compositor> _requestedCommits = new HashSet<Compositor>();

	private readonly Dictionary<Compositor, Batch> _pendingCompositionBatches = new Dictionary<Compositor, Batch>();

	private List<Action>? _invokeOnRenderCallbacks;

	private readonly Stack<List<Action>> _invokeOnRenderCallbackListPool = new Stack<List<Action>>();

	private DispatcherTimer _animationsTimer = new DispatcherTimer(DispatcherPriority.Render)
	{
		Interval = TimeSpan.FromMilliseconds(16.0)
	};

	private Dictionary<object, TopLevelInfo> _topLevels = new Dictionary<object, TopLevelInfo>();

	public IGlobalClock Clock => _clock;

	public static MediaContext Instance
	{
		get
		{
			MediaContext mediaContext = AvaloniaLocator.Current.GetService<MediaContext>();
			if (mediaContext == null)
			{
				AvaloniaLocator.CurrentMutable.Bind<MediaContext>().ToConstant(mediaContext = new MediaContext());
			}
			return mediaContext;
		}
	}

	public void RequestAnimationFrame(Action<TimeSpan> action)
	{
		_clock.RequestAnimationFrame(action);
	}

	private Batch CommitCompositor(Compositor compositor)
	{
		Batch commit = compositor.Commit();
		_requestedCommits.Remove(compositor);
		_pendingCompositionBatches[compositor] = commit;
		commit.Processed.ContinueWith(delegate
		{
			Dispatcher.UIThread.Post(delegate
			{
				CompositionBatchFinished(compositor, commit);
			}, DispatcherPriority.Send);
		});
		return commit;
	}

	private void CompositionBatchFinished(Compositor compositor, Batch batch)
	{
		if (_pendingCompositionBatches.TryGetValue(compositor, out Batch value) && value == batch)
		{
			_pendingCompositionBatches.Remove(compositor);
		}
		if (_pendingCompositionBatches.Count != 0)
		{
			return;
		}
		_animationsAreWaitingForComposition = false;
		if (_scheduleCommitOnLastCompositionBatchCompletion)
		{
			_scheduleCommitOnLastCompositionBatchCompletion = false;
			if (!CommitCompositorsWithThrottling())
			{
				ScheduleRenderForAnimationsIfNeeded();
			}
		}
		else
		{
			ScheduleRenderForAnimationsIfNeeded();
		}
	}

	private void ScheduleRenderForAnimationsIfNeeded()
	{
		if (_clock.HasSubscriptions)
		{
			ScheduleRender(now: false);
		}
	}

	private bool CommitCompositorsWithThrottling()
	{
		if (_pendingCompositionBatches.Count > 0)
		{
			_scheduleCommitOnLastCompositionBatchCompletion = true;
			return true;
		}
		if (_requestedCommits.Count == 0)
		{
			return false;
		}
		Compositor[] array = _requestedCommits.ToArray();
		foreach (Compositor compositor in array)
		{
			CommitCompositor(compositor);
		}
		_requestedCommits.Clear();
		return true;
	}

	private void SyncCommit(Compositor compositor, bool waitFullRender)
	{
		if (AvaloniaLocator.Current.GetService<IPlatformRenderInterface>() == null)
		{
			return;
		}
		if (compositor != null && !compositor.UseUiThreadForSynchronousCommits)
		{
			IRenderLoop loop = compositor.Loop;
			if (loop != null && loop.RunsInBackground)
			{
				Batch batch = CommitCompositor(compositor);
				(waitFullRender ? batch.Rendered : batch.Processed).Wait();
				return;
			}
		}
		CommitCompositor(compositor);
		compositor.Server.Render();
	}

	public void ImmediateRenderRequested(CompositionTarget target)
	{
		SyncCommit(target.Compositor, waitFullRender: true);
	}

	public void SyncDisposeCompositionTarget(CompositionTarget compositionTarget)
	{
		compositionTarget.Dispose();
		SyncCommit(compositionTarget.Compositor, waitFullRender: false);
	}

	void ICompositorScheduler.CommitRequested(Compositor compositor)
	{
		if (_requestedCommits.Add(compositor))
		{
			ScheduleRender(now: true);
		}
	}

	private MediaContext()
	{
		_render = Render;
		_inputMarkerHandler = InputMarkerHandler;
		_clock = new MediaContextClock(this);
		_animationsTimer.Tick += delegate
		{
			_animationsTimer.Stop();
			ScheduleRender(now: false);
		};
	}

	private void ScheduleRender(bool now)
	{
		if (_nextRenderOp != null)
		{
			if (now)
			{
				_nextRenderOp.Priority = DispatcherPriority.Render;
			}
			return;
		}
		DispatcherPriority priority = DispatcherPriority.Render;
		if (_inputMarkerOp == null)
		{
			_inputMarkerOp = Dispatcher.UIThread.InvokeAsync(_inputMarkerHandler, DispatcherPriority.Input);
			_inputMarkerAddedAt = _time.Elapsed;
		}
		else if (!now && (_time.Elapsed - _inputMarkerAddedAt).TotalSeconds > 1.0)
		{
			priority = DispatcherPriority.Input;
		}
		_nextRenderOp = Dispatcher.UIThread.InvokeAsync(_render, priority);
	}

	private void InputMarkerHandler()
	{
		_inputMarkerOp = null;
	}

	private void Render()
	{
		try
		{
			_isRendering = true;
			RenderCore();
		}
		finally
		{
			_nextRenderOp = null;
			_isRendering = false;
		}
	}

	private void RenderCore()
	{
		TimeSpan elapsed = _time.Elapsed;
		if (!_animationsAreWaitingForComposition)
		{
			_clock.Pulse(elapsed);
		}
		for (int i = 0; i < 10; i++)
		{
			FireInvokeOnRenderCallbacks();
			if (!_clock.HasNewSubscriptions)
			{
				break;
			}
			_clock.PulseNewSubscriptions();
		}
		if (_requestedCommits.Count > 0 || _clock.HasSubscriptions)
		{
			_animationsAreWaitingForComposition = CommitCompositorsWithThrottling();
			if (!_animationsAreWaitingForComposition && _clock.HasSubscriptions)
			{
				_animationsTimer.Start();
			}
		}
	}

	public bool IsTopLevelActive(object key)
	{
		return _topLevels.ContainsKey(key);
	}

	public void AddTopLevel(object key, ILayoutManager layoutManager, IRenderer renderer)
	{
		if (!_topLevels.ContainsKey(key))
		{
			CompositingRenderer compositingRenderer = (CompositingRenderer)renderer;
			_topLevels.Add(key, new TopLevelInfo(compositingRenderer.Compositor, compositingRenderer, layoutManager));
			compositingRenderer.Start();
			ScheduleRender(now: true);
		}
	}

	public void RemoveTopLevel(object key)
	{
		if (_topLevels.TryGetValue(key, out TopLevelInfo value))
		{
			_topLevels.Remove(key);
			value.Renderer.Stop();
		}
	}

	private void FireInvokeOnRenderCallbacks()
	{
		int num = 0;
		int num2 = _invokeOnRenderCallbacks?.Count ?? 0;
		while (true)
		{
			if (num2 > 0)
			{
				num++;
				if (num > 153)
				{
					throw new InvalidOperationException("Infinite layout loop detected");
				}
				List<Action> invokeOnRenderCallbacks = _invokeOnRenderCallbacks;
				_invokeOnRenderCallbacks = null;
				for (int i = 0; i < num2; i++)
				{
					invokeOnRenderCallbacks[i]();
				}
				invokeOnRenderCallbacks.Clear();
				_invokeOnRenderCallbackListPool.Push(invokeOnRenderCallbacks);
				num2 = _invokeOnRenderCallbacks?.Count ?? 0;
			}
			else
			{
				num2 = _invokeOnRenderCallbacks?.Count ?? 0;
				if (num2 <= 0)
				{
					break;
				}
			}
		}
	}

	public void BeginInvokeOnRender(Action callback)
	{
		if (_invokeOnRenderCallbacks == null)
		{
			_invokeOnRenderCallbacks = ((_invokeOnRenderCallbackListPool.Count > 0) ? _invokeOnRenderCallbackListPool.Pop() : new List<Action>());
		}
		_invokeOnRenderCallbacks.Add(callback);
		if (!_isRendering)
		{
			ScheduleRender(now: true);
		}
	}
}
