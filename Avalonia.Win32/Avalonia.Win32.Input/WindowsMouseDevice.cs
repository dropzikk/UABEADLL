using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32.Input;

internal class WindowsMouseDevice : MouseDevice
{
	internal class WindowsMousePointer : Pointer
	{
		private WindowsMousePointer()
			: base(Pointer.GetNextFreeId(), PointerType.Mouse, isPrimary: true)
		{
		}

		public static WindowsMousePointer CreatePointer(out WindowsMousePointer pointer)
		{
			return pointer = new WindowsMousePointer();
		}

		protected override void PlatformCapture(IInputElement? element)
		{
			IntPtr? intPtr = (TopLevel.GetTopLevel(element as Visual)?.PlatformImpl as WindowImpl)?.Handle.Handle;
			if (intPtr.HasValue && intPtr != IntPtr.Zero)
			{
				UnmanagedMethods.SetCapture(intPtr.Value);
			}
			else
			{
				UnmanagedMethods.ReleaseCapture();
			}
		}
	}

	private readonly IPointer _pointer;

	public WindowsMouseDevice()
		: base(WindowsMousePointer.CreatePointer(out WindowsMousePointer pointer))
	{
		_pointer = pointer;
	}

	internal void Capture(IInputElement? control)
	{
		_pointer.Capture(control);
	}
}
