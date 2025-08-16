using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32.OpenGl;

internal class WglGdiResourceManager
{
	private class GetDCOp
	{
		public readonly IntPtr Window;

		public readonly TaskCompletionSource<IntPtr> Result;

		public GetDCOp(IntPtr window, TaskCompletionSource<IntPtr> result)
		{
			Window = window;
			Result = result;
		}
	}

	private class ReleaseDCOp
	{
		public readonly IntPtr Window;

		public readonly IntPtr DC;

		public readonly TaskCompletionSource<object?> Result;

		public ReleaseDCOp(IntPtr window, IntPtr dc, TaskCompletionSource<object?> result)
		{
			Window = window;
			DC = dc;
			Result = result;
		}
	}

	private class CreateWindowOp
	{
		public readonly TaskCompletionSource<IntPtr> Result;

		public CreateWindowOp(TaskCompletionSource<IntPtr> result)
		{
			Result = result;
		}
	}

	private class DestroyWindowOp
	{
		public readonly IntPtr Window;

		public readonly TaskCompletionSource<object?> Result;

		public DestroyWindowOp(IntPtr window, TaskCompletionSource<object?> result)
		{
			Window = window;
			Result = result;
		}
	}

	private static readonly Queue<object> s_queue;

	private static readonly AutoResetEvent s_event;

	private static readonly ushort s_windowClass;

	private static readonly UnmanagedMethods.WndProc s_wndProcDelegate;

	private static void Worker()
	{
		while (true)
		{
			s_event.WaitOne();
			lock (s_queue)
			{
				if (s_queue.Count != 0)
				{
					object obj = s_queue.Dequeue();
					if (obj is GetDCOp getDCOp)
					{
						getDCOp.Result.TrySetResult(UnmanagedMethods.GetDC(getDCOp.Window));
					}
					else if (obj is ReleaseDCOp releaseDCOp)
					{
						UnmanagedMethods.ReleaseDC(releaseDCOp.Window, releaseDCOp.DC);
						releaseDCOp.Result.SetResult(null);
					}
					else if (obj is CreateWindowOp createWindowOp)
					{
						createWindowOp.Result.TrySetResult(UnmanagedMethods.CreateWindowEx(0, s_windowClass, null, 13565952u, 0, 0, 640, 480, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero));
					}
					else if (obj is DestroyWindowOp destroyWindowOp)
					{
						UnmanagedMethods.DestroyWindow(destroyWindowOp.Window);
						destroyWindowOp.Result.TrySetResult(null);
					}
				}
			}
		}
	}

	static WglGdiResourceManager()
	{
		s_queue = new Queue<object>();
		s_event = new AutoResetEvent(initialState: false);
		s_wndProcDelegate = WndProc;
		UnmanagedMethods.WNDCLASSEX lpwcx = new UnmanagedMethods.WNDCLASSEX
		{
			cbSize = Marshal.SizeOf<UnmanagedMethods.WNDCLASSEX>(),
			hInstance = UnmanagedMethods.GetModuleHandle(null),
			lpfnWndProc = s_wndProcDelegate,
			lpszClassName = "AvaloniaGlWindow-" + Guid.NewGuid(),
			style = 32
		};
		s_windowClass = UnmanagedMethods.RegisterClassEx(ref lpwcx);
		Thread thread = new Thread(Worker);
		thread.IsBackground = true;
		thread.Name = "Win32 OpenGL HDC manager";
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
	}

	private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
	}

	public static IntPtr CreateOffscreenWindow()
	{
		TaskCompletionSource<IntPtr> taskCompletionSource = new TaskCompletionSource<IntPtr>();
		lock (s_queue)
		{
			s_queue.Enqueue(new CreateWindowOp(taskCompletionSource));
		}
		s_event.Set();
		return taskCompletionSource.Task.Result;
	}

	public static IntPtr GetDC(IntPtr hWnd)
	{
		TaskCompletionSource<IntPtr> taskCompletionSource = new TaskCompletionSource<IntPtr>();
		lock (s_queue)
		{
			s_queue.Enqueue(new GetDCOp(hWnd, taskCompletionSource));
		}
		s_event.Set();
		return taskCompletionSource.Task.Result;
	}

	public static void ReleaseDC(IntPtr hWnd, IntPtr hDC)
	{
		TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
		lock (s_queue)
		{
			s_queue.Enqueue(new ReleaseDCOp(hWnd, hDC, taskCompletionSource));
		}
		s_event.Set();
		taskCompletionSource.Task.Wait();
	}

	public static void DestroyWindow(IntPtr hWnd)
	{
		TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
		lock (s_queue)
		{
			s_queue.Enqueue(new DestroyWindowOp(hWnd, taskCompletionSource));
		}
		s_event.Set();
		taskCompletionSource.Task.Wait();
	}
}
