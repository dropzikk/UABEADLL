using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Platform.Interop;

namespace Avalonia.X11.NativeDialogs;

internal static class Glib
{
	private delegate bool timeout_callback(IntPtr data);

	private class ConnectedSignal : IDisposable
	{
		private readonly IntPtr _instance;

		private GCHandle _handle;

		private readonly ulong _id;

		public ConnectedSignal(IntPtr instance, GCHandle handle, ulong id)
		{
			_instance = instance;
			g_object_ref(instance);
			_handle = handle;
			_id = id;
		}

		public void Dispose()
		{
			if (_handle.IsAllocated)
			{
				g_signal_handler_disconnect(_instance, _id);
				g_object_unref(_instance);
				_handle.Free();
			}
		}
	}

	private const string GlibName = "libglib-2.0.so.0";

	private const string GObjectName = "libgobject-2.0.so.0";

	private static readonly timeout_callback s_pinnedHandler;

	[DllImport("libglib-2.0.so.0")]
	public unsafe static extern void g_slist_free(GSList* data);

	[DllImport("libgobject-2.0.so.0")]
	private static extern void g_object_ref(IntPtr instance);

	[DllImport("libgobject-2.0.so.0")]
	private static extern ulong g_signal_connect_object(IntPtr instance, Utf8Buffer signal, IntPtr handler, IntPtr userData, int flags);

	[DllImport("libgobject-2.0.so.0")]
	private static extern void g_object_unref(IntPtr instance);

	[DllImport("libgobject-2.0.so.0")]
	private static extern ulong g_signal_handler_disconnect(IntPtr instance, ulong connectionId);

	[DllImport("libglib-2.0.so.0")]
	private static extern ulong g_timeout_add_full(int prio, uint interval, timeout_callback callback, IntPtr data, IntPtr destroy);

	public static IDisposable ConnectSignal<T>(IntPtr obj, string name, T handler)
	{
		GCHandle handle = GCHandle.Alloc(handler);
		IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(handler);
		using Utf8Buffer signal = new Utf8Buffer(name);
		ulong num = g_signal_connect_object(obj, signal, functionPointerForDelegate, IntPtr.Zero, 0);
		if (num == 0L)
		{
			throw new ArgumentException("Unable to connect to signal " + name);
		}
		return new ConnectedSignal(obj, handle, num);
	}

	private static bool TimeoutHandler(IntPtr data)
	{
		GCHandle gCHandle = GCHandle.FromIntPtr(data);
		if (!((Func<bool>)gCHandle.Target)())
		{
			gCHandle.Free();
			return false;
		}
		return true;
	}

	static Glib()
	{
		s_pinnedHandler = TimeoutHandler;
	}

	private static void AddTimeout(int priority, uint interval, Func<bool> callback)
	{
		GCHandle value = GCHandle.Alloc(callback);
		g_timeout_add_full(priority, interval, s_pinnedHandler, GCHandle.ToIntPtr(value), IntPtr.Zero);
	}

	public static Task<T> RunOnGlibThread<T>(Func<T> action)
	{
		TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
		AddTimeout(0, 0u, delegate
		{
			try
			{
				tcs.SetResult(action());
			}
			catch (Exception exception)
			{
				tcs.TrySetException(exception);
			}
			return false;
		});
		return tcs.Task;
	}
}
