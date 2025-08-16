using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Threading;

namespace Avalonia.X11;

internal class X11PlatformThreading : IControlledDispatcherImpl, IDispatcherImplWithPendingInput, IDispatcherImpl
{
	public delegate void EventHandler(ref XEvent xev);

	[StructLayout(LayoutKind.Explicit)]
	private struct epoll_data
	{
		[FieldOffset(0)]
		public IntPtr ptr;

		[FieldOffset(0)]
		public int fd;

		[FieldOffset(0)]
		public uint u32;

		[FieldOffset(0)]
		public ulong u64;
	}

	private struct epoll_event
	{
		public uint events;

		public epoll_data data;
	}

	private enum EventCodes
	{
		X11 = 1,
		Signal
	}

	private readonly AvaloniaX11Platform _platform;

	private readonly IntPtr _display;

	private readonly Dictionary<IntPtr, EventHandler> _eventHandlers;

	private Thread _mainThread;

	private const int EPOLLIN = 1;

	private const int EPOLL_CTL_ADD = 1;

	private const int O_NONBLOCK = 2048;

	private int _sigread;

	private int _sigwrite;

	private object _lock = new object();

	private bool _signaled;

	private bool _wakeupRequested;

	private long? _nextTimer;

	private int _epoll;

	private Stopwatch _clock = Stopwatch.StartNew();

	public bool CurrentThreadIsLoopThread => Thread.CurrentThread == _mainThread;

	public long Now => (int)_clock.ElapsedMilliseconds;

	public bool CanQueryPendingInput => true;

	public bool HasPendingInput
	{
		get
		{
			if (!_platform.EventGrouperDispatchQueue.HasJobs)
			{
				return XLib.XPending(_display) != 0;
			}
			return true;
		}
	}

	public event Action Signaled;

	public event Action Timer;

	[DllImport("libc")]
	private static extern int epoll_create1(int size);

	[DllImport("libc")]
	private static extern int epoll_ctl(int epfd, int op, int fd, ref epoll_event __event);

	[DllImport("libc")]
	private unsafe static extern int epoll_wait(int epfd, epoll_event* events, int maxevents, int timeout);

	[DllImport("libc")]
	private unsafe static extern int pipe2(int* fds, int flags);

	[DllImport("libc")]
	private unsafe static extern IntPtr write(int fd, void* buf, IntPtr count);

	[DllImport("libc")]
	private unsafe static extern IntPtr read(int fd, void* buf, IntPtr count);

	public unsafe X11PlatformThreading(AvaloniaX11Platform platform)
	{
		_platform = platform;
		_display = platform.Display;
		_eventHandlers = platform.Windows;
		_mainThread = Thread.CurrentThread;
		int fd = XLib.XConnectionNumber(_display);
		epoll_event __event = new epoll_event
		{
			events = 1u,
			data = 
			{
				u32 = 1u
			}
		};
		_epoll = epoll_create1(0);
		if (_epoll == -1)
		{
			throw new X11Exception("epoll_create1 failed");
		}
		if (epoll_ctl(_epoll, 1, fd, ref __event) == -1)
		{
			throw new X11Exception("Unable to attach X11 connection handle to epoll");
		}
		int* ptr = stackalloc int[2];
		pipe2(ptr, 2048);
		_sigread = *ptr;
		_sigwrite = ptr[1];
		__event = new epoll_event
		{
			events = 1u,
			data = 
			{
				u32 = 2u
			}
		};
		if (epoll_ctl(_epoll, 1, _sigread, ref __event) == -1)
		{
			throw new X11Exception("Unable to attach signal pipe to epoll");
		}
	}

	private void CheckSignaled()
	{
		lock (_lock)
		{
			if (!_signaled)
			{
				return;
			}
			_signaled = false;
		}
		this.Signaled?.Invoke();
	}

	private unsafe void HandleX11(CancellationToken cancellationToken)
	{
		while (XLib.XPending(_display) != 0 && !cancellationToken.IsCancellationRequested)
		{
			XLib.XNextEvent(_display, out var xevent);
			if (XLib.XFilterEvent(ref xevent, IntPtr.Zero))
			{
				continue;
			}
			if (xevent.type == XEventName.GenericEvent)
			{
				XLib.XGetEventData(_display, &xevent.GenericEventCookie);
			}
			try
			{
				EventHandler value;
				if (xevent.type == XEventName.GenericEvent)
				{
					if (_platform.XI2 != null && _platform.Info.XInputOpcode == xevent.GenericEventCookie.extension)
					{
						_platform.XI2.OnEvent((XIEvent*)xevent.GenericEventCookie.data);
					}
				}
				else if (_eventHandlers.TryGetValue(xevent.AnyEvent.window, out value))
				{
					value(ref xevent);
				}
			}
			finally
			{
				if (xevent.type == XEventName.GenericEvent && xevent.GenericEventCookie.data != null)
				{
					XLib.XFreeEventData(_display, &xevent.GenericEventCookie);
				}
			}
		}
	}

	public unsafe void RunLoop(CancellationToken cancellationToken)
	{
		epoll_event epoll_event = default(epoll_event);
		while (!cancellationToken.IsCancellationRequested)
		{
			long elapsedMilliseconds = _clock.ElapsedMilliseconds;
			if (_nextTimer.HasValue && elapsedMilliseconds > _nextTimer.Value)
			{
				this.Timer?.Invoke();
			}
			if (cancellationToken.IsCancellationRequested)
			{
				break;
			}
			XLib.XFlush(_display);
			if (XLib.XPending(_display) == 0)
			{
				elapsedMilliseconds = _clock.ElapsedMilliseconds;
				if (_nextTimer < elapsedMilliseconds)
				{
					continue;
				}
				long val = ((!_nextTimer.HasValue) ? (-1) : Math.Max(1L, _nextTimer.Value - elapsedMilliseconds));
				epoll_wait(_epoll, &epoll_event, 1, (int)Math.Min(2147483647L, val));
				int num = 0;
				while (read(_sigread, &num, new IntPtr(4)).ToInt64() > 0)
				{
				}
				lock (_lock)
				{
					_wakeupRequested = false;
				}
			}
			if (cancellationToken.IsCancellationRequested)
			{
				break;
			}
			CheckSignaled();
			HandleX11(cancellationToken);
			while (_platform.EventGrouperDispatchQueue.HasJobs)
			{
				CheckSignaled();
				_platform.EventGrouperDispatchQueue.DispatchNext();
			}
		}
	}

	private unsafe void Wakeup()
	{
		lock (_lock)
		{
			if (!_wakeupRequested)
			{
				_wakeupRequested = true;
				int num = 0;
				write(_sigwrite, &num, new IntPtr(1));
			}
		}
	}

	public void Signal()
	{
		lock (_lock)
		{
			if (!_signaled)
			{
				_signaled = true;
				Wakeup();
			}
		}
	}

	public void UpdateTimer(long? dueTimeInMs)
	{
		_nextTimer = dueTimeInMs;
		if (_nextTimer.HasValue)
		{
			Wakeup();
		}
	}
}
