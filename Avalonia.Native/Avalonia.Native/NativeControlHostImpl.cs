using System;
using Avalonia.Controls.Platform;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class NativeControlHostImpl : IDisposable, INativeControlHostImpl
{
	private class DestroyableNSView : INativeControlHostDestroyableControlHandle, IPlatformHandle
	{
		private IAvnNativeControlHost _impl;

		private IntPtr _nsView;

		public IntPtr Handle => _nsView;

		public string HandleDescriptor => "NSView";

		public DestroyableNSView(IAvnNativeControlHost impl)
		{
			_impl = impl.CloneReference();
			_nsView = _impl.CreateDefaultChild(IntPtr.Zero);
		}

		public void Destroy()
		{
			if (_impl != null)
			{
				_impl.DestroyDefaultChild(_nsView);
				_impl.Dispose();
				_impl = null;
				_nsView = IntPtr.Zero;
			}
		}
	}

	private class Attachment : INativeControlHostControlTopLevelAttachment, IDisposable
	{
		private IAvnNativeControlHostTopLevelAttachment _native;

		private NativeControlHostImpl _attachedTo;

		public INativeControlHostImpl AttachedTo
		{
			get
			{
				return _attachedTo;
			}
			set
			{
				NativeControlHostImpl nativeControlHostImpl = (NativeControlHostImpl)value;
				if (nativeControlHostImpl == null)
				{
					_native.AttachTo(null);
				}
				else
				{
					_native.AttachTo(nativeControlHostImpl._host);
				}
				_attachedTo = nativeControlHostImpl;
			}
		}

		public IPlatformHandle GetParentHandle()
		{
			return new PlatformHandle(_native.ParentHandle, "NSView");
		}

		public Attachment(IAvnNativeControlHostTopLevelAttachment native)
		{
			_native = native;
		}

		public void Dispose()
		{
			if (_native != null)
			{
				_native.ReleaseChild();
				_native.Dispose();
				_native = null;
			}
		}

		public bool IsCompatibleWith(INativeControlHostImpl host)
		{
			return host is NativeControlHostImpl;
		}

		public void HideWithSize(Size size)
		{
			_native.HideWithSize(Math.Max(1f, (float)size.Width), Math.Max(1f, (float)size.Height));
		}

		public void ShowInBounds(Rect bounds)
		{
			if (_attachedTo == null)
			{
				throw new InvalidOperationException("Native control isn't attached to a toplevel");
			}
			bounds = new Rect(bounds.X, bounds.Y, Math.Max(1.0, bounds.Width), Math.Max(1.0, bounds.Height));
			_native.ShowInBounds((float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height);
		}

		public void InitWithChild(IPlatformHandle handle)
		{
			_native.InitializeWithChildHandle(handle.Handle);
		}
	}

	private IAvnNativeControlHost _host;

	public NativeControlHostImpl(IAvnNativeControlHost host)
	{
		_host = host;
	}

	public void Dispose()
	{
		_host?.Dispose();
		_host = null;
	}

	public INativeControlHostDestroyableControlHandle CreateDefaultChild(IPlatformHandle parent)
	{
		return new DestroyableNSView(_host);
	}

	public INativeControlHostControlTopLevelAttachment CreateNewAttachment(Func<IPlatformHandle, IPlatformHandle> create)
	{
		Attachment attachment = new Attachment(_host.CreateAttachment());
		try
		{
			IPlatformHandle handle = create(attachment.GetParentHandle());
			attachment.InitWithChild(handle);
			attachment.AttachedTo = this;
			return attachment;
		}
		catch
		{
			attachment.Dispose();
			throw;
		}
	}

	public INativeControlHostControlTopLevelAttachment CreateNewAttachment(IPlatformHandle handle)
	{
		Attachment attachment = new Attachment(_host.CreateAttachment());
		attachment.InitWithChild(handle);
		attachment.AttachedTo = this;
		return attachment;
	}

	public bool IsCompatibleWith(IPlatformHandle handle)
	{
		return handle.HandleDescriptor == "NSView";
	}
}
