using System;
using Avalonia.Controls.Platform;
using Avalonia.Platform;

namespace Avalonia.X11;

internal class X11NativeControlHost : INativeControlHostImpl
{
	private class DumbWindow : INativeControlHostDestroyableControlHandle, IPlatformHandle
	{
		private readonly IntPtr _display;

		public IntPtr Handle { get; private set; }

		public string HandleDescriptor => "XID";

		public DumbWindow(X11Info x11, IntPtr? parent = null)
		{
			_display = x11.Display;
			XSetWindowAttributes attributes = new XSetWindowAttributes
			{
				backing_store = 1,
				bit_gravity = Gravity.NorthWestGravity,
				win_gravity = Gravity.NorthWestGravity
			};
			parent = parent ?? XLib.XDefaultRootWindow(x11.Display);
			Handle = XLib.XCreateWindow(_display, parent.Value, 0, 0, 1, 1, 0, 0, 1, IntPtr.Zero, new UIntPtr(122u), ref attributes);
		}

		public void Destroy()
		{
			if (Handle != IntPtr.Zero)
			{
				XLib.XDestroyWindow(_display, Handle);
				Handle = IntPtr.Zero;
			}
		}
	}

	private class Attachment : INativeControlHostControlTopLevelAttachment, IDisposable
	{
		private readonly IntPtr _display;

		private readonly IntPtr _orphanedWindow;

		private DumbWindow _holder;

		private IPlatformHandle _child;

		private X11NativeControlHost _attachedTo;

		private bool _mapped;

		public INativeControlHostImpl AttachedTo
		{
			get
			{
				return _attachedTo;
			}
			set
			{
				CheckDisposed();
				_attachedTo = (X11NativeControlHost)value;
				if (value == null)
				{
					_mapped = false;
					XLib.XUnmapWindow(_display, _holder.Handle);
					XLib.XReparentWindow(_display, _holder.Handle, _orphanedWindow, 0, 0);
				}
				else
				{
					XLib.XReparentWindow(_display, _holder.Handle, _attachedTo.Window.Handle.Handle, 0, 0);
				}
			}
		}

		public Attachment(IntPtr display, DumbWindow holder, IntPtr orphanedWindow, IPlatformHandle child)
		{
			_display = display;
			_orphanedWindow = orphanedWindow;
			_holder = holder;
			_child = child;
			XLib.XReparentWindow(_display, child.Handle, holder.Handle, 0, 0);
			XLib.XMapWindow(_display, child.Handle);
		}

		public void Dispose()
		{
			if (_child != null)
			{
				XLib.XReparentWindow(_display, _child.Handle, _orphanedWindow, 0, 0);
				_child = null;
			}
			_holder?.Destroy();
			_holder = null;
			_attachedTo = null;
		}

		private void CheckDisposed()
		{
			if (_child == null)
			{
				throw new ObjectDisposedException("X11 INativeControlHostControlTopLevelAttachment");
			}
		}

		public bool IsCompatibleWith(INativeControlHostImpl host)
		{
			return host is X11NativeControlHost;
		}

		public void HideWithSize(Size size)
		{
			if (_attachedTo != null && _child != null)
			{
				if (_mapped)
				{
					_mapped = false;
					XLib.XUnmapWindow(_display, _holder.Handle);
				}
				size *= _attachedTo.Window.RenderScaling;
				XLib.XResizeWindow(_display, _child.Handle, Math.Max(1, (int)size.Width), Math.Max(1, (int)size.Height));
			}
		}

		public void ShowInBounds(Rect bounds)
		{
			CheckDisposed();
			if (_attachedTo == null)
			{
				throw new InvalidOperationException("The control isn't currently attached to a toplevel");
			}
			bounds *= _attachedTo.Window.RenderScaling;
			PixelRect pixelRect = new PixelRect((int)bounds.X, (int)bounds.Y, Math.Max(1, (int)bounds.Width), Math.Max(1, (int)bounds.Height));
			XLib.XMoveResizeWindow(_display, _child.Handle, 0, 0, pixelRect.Width, pixelRect.Height);
			XLib.XMoveResizeWindow(_display, _holder.Handle, pixelRect.X, pixelRect.Y, pixelRect.Width, pixelRect.Height);
			if (!_mapped)
			{
				XLib.XMapWindow(_display, _holder.Handle);
				XLib.XRaiseWindow(_display, _holder.Handle);
				_mapped = true;
			}
		}
	}

	private readonly AvaloniaX11Platform _platform;

	public X11Window Window { get; }

	public X11NativeControlHost(AvaloniaX11Platform platform, X11Window window)
	{
		_platform = platform;
		Window = window;
	}

	public INativeControlHostDestroyableControlHandle CreateDefaultChild(IPlatformHandle parent)
	{
		DumbWindow result = new DumbWindow(_platform.Info, null);
		XLib.XSync(_platform.Display, discard: false);
		return result;
	}

	public INativeControlHostControlTopLevelAttachment CreateNewAttachment(Func<IPlatformHandle, IPlatformHandle> create)
	{
		DumbWindow dumbWindow = new DumbWindow(_platform.Info, Window.Handle.Handle);
		Attachment attachment = null;
		try
		{
			IPlatformHandle child = create(dumbWindow);
			attachment = new Attachment(_platform.Display, dumbWindow, _platform.OrphanedWindow, child);
			attachment.AttachedTo = this;
			return attachment;
		}
		catch
		{
			attachment?.Dispose();
			dumbWindow?.Destroy();
			throw;
		}
	}

	public INativeControlHostControlTopLevelAttachment CreateNewAttachment(IPlatformHandle handle)
	{
		if (!IsCompatibleWith(handle))
		{
			throw new ArgumentException(handle.HandleDescriptor + " is not compatible with the current window", "handle");
		}
		return new Attachment(_platform.Display, new DumbWindow(_platform.Info, Window.Handle.Handle), _platform.OrphanedWindow, handle)
		{
			AttachedTo = this
		};
	}

	public bool IsCompatibleWith(IPlatformHandle handle)
	{
		return handle.HandleDescriptor == "XID";
	}
}
