using System;
using System.Runtime.InteropServices;
using Avalonia.OpenGL.Egl;
using Avalonia.SourceGenerator;

namespace Avalonia.OpenGL.Angle;

internal class Win32AngleEglInterface : EglInterface
{
	private unsafe delegate* unmanaged[Stdcall]<int, IntPtr, int*, IntPtr> _addr_CreateDeviceANGLE;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, void> _addr_ReleaseDeviceANGLE;

	public unsafe bool IsCreateDeviceANGLEAvailable => _addr_CreateDeviceANGLE != (delegate* unmanaged[Stdcall]<int, IntPtr, int*, IntPtr>)null;

	public unsafe bool IsReleaseDeviceANGLEAvailable => _addr_ReleaseDeviceANGLE != (delegate* unmanaged[Stdcall]<IntPtr, void>)null;

	[DllImport("av_libGLESv2.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr EGL_GetProcAddress(string proc);

	public Win32AngleEglInterface()
		: this(LoadAngle())
	{
	}

	private Win32AngleEglInterface(Func<string, IntPtr> getProcAddress)
		: base(getProcAddress)
	{
		Initialize(getProcAddress);
	}

	[GetProcAddress("eglCreateDeviceANGLE", true)]
	public unsafe IntPtr CreateDeviceANGLE(int deviceType, IntPtr nativeDevice, int[]? attribs)
	{
		if (_addr_CreateDeviceANGLE == (delegate* unmanaged[Stdcall]<int, IntPtr, int*, IntPtr>)null)
		{
			throw new EntryPointNotFoundException("CreateDeviceANGLE");
		}
		fixed (int* ptr = attribs)
		{
			return _addr_CreateDeviceANGLE(deviceType, nativeDevice, ptr);
		}
	}

	[GetProcAddress("eglReleaseDeviceANGLE", true)]
	public unsafe void ReleaseDeviceANGLE(IntPtr device)
	{
		if (_addr_ReleaseDeviceANGLE == (delegate* unmanaged[Stdcall]<IntPtr, void>)null)
		{
			throw new EntryPointNotFoundException("ReleaseDeviceANGLE");
		}
		_addr_ReleaseDeviceANGLE(device);
	}

	private static Func<string, IntPtr> LoadAngle()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			if (EGL_GetProcAddress("eglGetPlatformDisplayEXT") == IntPtr.Zero)
			{
				throw new OpenGlException("libegl.dll doesn't have eglGetPlatformDisplayEXT entry point");
			}
			return EGL_GetProcAddress;
		}
		throw new PlatformNotSupportedException();
	}

	private unsafe void Initialize(Func<string, IntPtr> getProcAddress)
	{
		IntPtr zero = IntPtr.Zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglCreateDeviceANGLE");
		_addr_CreateDeviceANGLE = (delegate* unmanaged[Stdcall]<int, IntPtr, int*, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglReleaseDeviceANGLE");
		_addr_ReleaseDeviceANGLE = (delegate* unmanaged[Stdcall]<IntPtr, void>)(void*)zero;
	}
}
