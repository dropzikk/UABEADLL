using System;
using System.Runtime.InteropServices;

namespace Avalonia.OpenGL.Egl;

public class EglSurface : SafeHandle
{
	private readonly EglDisplay _display;

	private readonly EglInterface _egl;

	public override bool IsInvalid => handle == IntPtr.Zero;

	public EglSurface(EglDisplay display, IntPtr surface)
		: base(surface, ownsHandle: true)
	{
		_display = display;
		_egl = display.EglInterface;
	}

	protected override bool ReleaseHandle()
	{
		using (_display.Lock())
		{
			_egl.DestroySurface(_display.Handle, handle);
		}
		return true;
	}

	public void SwapBuffers()
	{
		_egl.SwapBuffers(_display.Handle, handle);
	}
}
