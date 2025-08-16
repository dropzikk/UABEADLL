using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class Win32NativeControlHost : INativeControlHostImpl
{
	private class DumbWindow : IDisposable, INativeControlHostDestroyableControlHandle, IPlatformHandle
	{
		private readonly UnmanagedMethods.WndProc _wndProcDelegate;

		private readonly string _className;

		public IntPtr Handle { get; }

		public string HandleDescriptor => "HWND";

		public void Destroy()
		{
			Dispose();
		}

		public DumbWindow(bool layered = false, IntPtr? parent = null)
		{
			_wndProcDelegate = WndProc;
			UnmanagedMethods.WNDCLASSEX lpwcx = new UnmanagedMethods.WNDCLASSEX
			{
				cbSize = Marshal.SizeOf<UnmanagedMethods.WNDCLASSEX>(),
				hInstance = UnmanagedMethods.GetModuleHandle(null),
				lpfnWndProc = _wndProcDelegate,
				lpszClassName = (_className = "AvaloniaDumbWindow-" + Guid.NewGuid())
			};
			ushort lpClassName = UnmanagedMethods.RegisterClassEx(ref lpwcx);
			Handle = UnmanagedMethods.CreateWindowEx(layered ? 524288 : 0, lpClassName, null, 1073741824u, 0, 0, 640, 480, parent ?? OffscreenParentWindow.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (Handle == IntPtr.Zero)
			{
				throw new InvalidOperationException("Unable to create child window for native control host. Application manifest with supported OS list might be required.");
			}
			if (layered)
			{
				UnmanagedMethods.SetLayeredWindowAttributes(Handle, 0u, byte.MaxValue, UnmanagedMethods.LayeredWindowFlags.LWA_ALPHA);
			}
		}

		private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
		}

		private void ReleaseUnmanagedResources()
		{
			UnmanagedMethods.DestroyWindow(Handle);
			UnmanagedMethods.UnregisterClass(_className, UnmanagedMethods.GetModuleHandle(null));
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~DumbWindow()
		{
			ReleaseUnmanagedResources();
		}
	}

	private class Win32NativeControlAttachment : INativeControlHostControlTopLevelAttachment, IDisposable
	{
		private DumbWindow? _holder;

		private IPlatformHandle? _child;

		private Win32NativeControlHost? _attachedTo;

		public INativeControlHostImpl? AttachedTo
		{
			get
			{
				return _attachedTo;
			}
			set
			{
				CheckDisposed();
				_attachedTo = value as Win32NativeControlHost;
				if (_attachedTo == null)
				{
					UnmanagedMethods.ShowWindow(_holder.Handle, UnmanagedMethods.ShowWindowCommand.Hide);
					UnmanagedMethods.SetParent(_holder.Handle, OffscreenParentWindow.Handle);
				}
				else
				{
					UnmanagedMethods.SetParent(_holder.Handle, _attachedTo.Window.Handle.Handle);
				}
			}
		}

		public Win32NativeControlAttachment(DumbWindow holder, IPlatformHandle child)
		{
			_holder = holder;
			_child = child;
			UnmanagedMethods.SetParent(child.Handle, _holder.Handle);
			UnmanagedMethods.ShowWindow(child.Handle, UnmanagedMethods.ShowWindowCommand.Show);
		}

		[MemberNotNull("_holder")]
		private void CheckDisposed()
		{
			if (_holder == null)
			{
				throw new ObjectDisposedException("Win32NativeControlAttachment");
			}
		}

		public void Dispose()
		{
			if (_child != null)
			{
				UnmanagedMethods.SetParent(_child.Handle, OffscreenParentWindow.Handle);
			}
			_holder?.Dispose();
			_holder = null;
			_child = null;
			_attachedTo = null;
		}

		public bool IsCompatibleWith(INativeControlHostImpl host)
		{
			return host is Win32NativeControlHost;
		}

		public void HideWithSize(Size size)
		{
			CheckDisposed();
			UnmanagedMethods.SetWindowPos(_holder.Handle, IntPtr.Zero, -100, -100, 1, 1, UnmanagedMethods.SetWindowPosFlags.SWP_HIDEWINDOW | UnmanagedMethods.SetWindowPosFlags.SWP_NOACTIVATE);
			if (_attachedTo != null && _child != null)
			{
				size *= _attachedTo.Window.RenderScaling;
				UnmanagedMethods.MoveWindow(_child.Handle, 0, 0, Math.Max(1, (int)size.Width), Math.Max(1, (int)size.Height), bRepaint: false);
			}
		}

		public unsafe void ShowInBounds(Rect bounds)
		{
			CheckDisposed();
			if (_attachedTo == null)
			{
				throw new InvalidOperationException("The control isn't currently attached to a toplevel");
			}
			bounds *= _attachedTo.Window.RenderScaling;
			PixelRect pixelRect = new PixelRect((int)bounds.X, (int)bounds.Y, Math.Max(1, (int)bounds.Width), Math.Max(1, (int)bounds.Height));
			if (_child != null)
			{
				UnmanagedMethods.MoveWindow(_child.Handle, 0, 0, pixelRect.Width, pixelRect.Height, bRepaint: true);
			}
			UnmanagedMethods.SetWindowPos(_holder.Handle, IntPtr.Zero, pixelRect.X, pixelRect.Y, pixelRect.Width, pixelRect.Height, UnmanagedMethods.SetWindowPosFlags.SWP_NOACTIVATE | UnmanagedMethods.SetWindowPosFlags.SWP_NOZORDER | UnmanagedMethods.SetWindowPosFlags.SWP_SHOWWINDOW);
			UnmanagedMethods.InvalidateRect(_attachedTo.Window.Handle.Handle, null, bErase: false);
		}
	}

	private readonly bool _useLayeredWindow;

	public WindowImpl Window { get; }

	public Win32NativeControlHost(WindowImpl window, bool useLayeredWindow)
	{
		_useLayeredWindow = useLayeredWindow;
		Window = window;
	}

	private void AssertCompatible(IPlatformHandle handle)
	{
		if (!IsCompatibleWith(handle))
		{
			throw new ArgumentException("Don't know what to do with " + handle.HandleDescriptor);
		}
	}

	public INativeControlHostDestroyableControlHandle CreateDefaultChild(IPlatformHandle parent)
	{
		AssertCompatible(parent);
		return new DumbWindow(layered: false, parent.Handle);
	}

	public INativeControlHostControlTopLevelAttachment CreateNewAttachment(Func<IPlatformHandle, IPlatformHandle> create)
	{
		DumbWindow dumbWindow = new DumbWindow(_useLayeredWindow, Window.Handle.Handle);
		Win32NativeControlAttachment win32NativeControlAttachment = null;
		try
		{
			IPlatformHandle child = create(dumbWindow);
			win32NativeControlAttachment = new Win32NativeControlAttachment(dumbWindow, child);
			win32NativeControlAttachment.AttachedTo = this;
			return win32NativeControlAttachment;
		}
		catch
		{
			win32NativeControlAttachment?.Dispose();
			dumbWindow.Destroy();
			throw;
		}
	}

	public INativeControlHostControlTopLevelAttachment CreateNewAttachment(IPlatformHandle handle)
	{
		AssertCompatible(handle);
		return new Win32NativeControlAttachment(new DumbWindow(_useLayeredWindow, Window.Handle.Handle), handle)
		{
			AttachedTo = this
		};
	}

	public bool IsCompatibleWith(IPlatformHandle handle)
	{
		return handle.HandleDescriptor == "HWND";
	}
}
