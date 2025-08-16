using System;
using System.Runtime.InteropServices;
using Avalonia.Compatibility;
using Avalonia.SourceGenerator;

namespace Avalonia.OpenGL.Egl;

public class EglInterface
{
	private unsafe delegate* unmanaged[Stdcall]<int> _addr_GetError;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr> _addr_GetDisplay;

	private unsafe delegate* unmanaged[Stdcall]<int, IntPtr, int*, IntPtr> _addr_GetPlatformDisplayExt;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, out int, out int, int> _addr_Initialize;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, void> _addr_Terminate;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr> _addr_GetProcAddress;

	private unsafe delegate* unmanaged[Stdcall]<int, int> _addr_BindApi;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, int*, out IntPtr, int, out int, int> _addr_ChooseConfig;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, int*, IntPtr> _addr_CreateContext;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int> _addr_DestroyContext;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int*, IntPtr> _addr_CreatePBufferSurface;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, IntPtr, int> _addr_MakeCurrent;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr> _addr_GetCurrentContext;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr> _addr_GetCurrentDisplay;

	private unsafe delegate* unmanaged[Stdcall]<int, IntPtr> _addr_GetCurrentSurface;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, void> _addr_DestroySurface;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, void> _addr_SwapBuffers;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, int*, IntPtr> _addr_CreateWindowSurface;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int, int> _addr_BindTexImage;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int, out int, int> _addr_GetConfigAttrib;

	private unsafe delegate* unmanaged[Stdcall]<int> _addr_WaitGL;

	private unsafe delegate* unmanaged[Stdcall]<int> _addr_WaitClient;

	private unsafe delegate* unmanaged[Stdcall]<int, int> _addr_WaitNative;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr> _addr_QueryStringNative;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr, IntPtr, int*, IntPtr> _addr_CreatePbufferFromClientBuffer;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, int, out IntPtr, int> _addr_QueryDisplayAttribExt;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, int, out IntPtr, int> _addr_QueryDeviceAttribExt;

	public unsafe bool IsGetPlatformDisplayExtAvailable => _addr_GetPlatformDisplayExt != (delegate* unmanaged[Stdcall]<int, IntPtr, int*, IntPtr>)null;

	public unsafe bool IsQueryDisplayAttribExtAvailable => _addr_QueryDisplayAttribExt != (delegate* unmanaged[Stdcall]<IntPtr, int, out IntPtr, int>)null;

	public unsafe bool IsQueryDeviceAttribExtAvailable => _addr_QueryDeviceAttribExt != (delegate* unmanaged[Stdcall]<IntPtr, int, out IntPtr, int>)null;

	public EglInterface(Func<string, IntPtr> getProcAddress)
	{
		Initialize(getProcAddress);
	}

	public EglInterface(string library)
		: this(Load(library))
	{
	}

	public EglInterface()
		: this(Load())
	{
	}

	private static Func<string, IntPtr> Load()
	{
		if (OperatingSystemEx.IsLinux())
		{
			return Load("libEGL.so.1");
		}
		if (OperatingSystemEx.IsAndroid())
		{
			return Load("libEGL.so");
		}
		throw new PlatformNotSupportedException();
	}

	private static Func<string, IntPtr> Load(string library)
	{
		IntPtr lib = NativeLibraryEx.Load(library);
		IntPtr address;
		return (string s) => (!NativeLibraryEx.TryGetExport(lib, s, out address)) ? ((IntPtr)0) : address;
	}

	[GetProcAddress("eglGetError")]
	public unsafe int GetError()
	{
		return _addr_GetError();
	}

	[GetProcAddress("eglGetDisplay")]
	public unsafe IntPtr GetDisplay(IntPtr nativeDisplay)
	{
		return _addr_GetDisplay(nativeDisplay);
	}

	[GetProcAddress("eglGetPlatformDisplayEXT", true)]
	public unsafe IntPtr GetPlatformDisplayExt(int platform, IntPtr nativeDisplay, int[]? attrs)
	{
		if (_addr_GetPlatformDisplayExt == (delegate* unmanaged[Stdcall]<int, IntPtr, int*, IntPtr>)null)
		{
			throw new EntryPointNotFoundException("GetPlatformDisplayExt");
		}
		fixed (int* ptr = attrs)
		{
			return _addr_GetPlatformDisplayExt(platform, nativeDisplay, ptr);
		}
	}

	[GetProcAddress("eglInitialize")]
	public unsafe bool Initialize(IntPtr display, out int major, out int minor)
	{
		return _addr_Initialize(display, out major, out minor) != 0;
	}

	[GetProcAddress("eglTerminate")]
	public unsafe void Terminate(IntPtr display)
	{
		_addr_Terminate(display);
	}

	[GetProcAddress("eglGetProcAddress")]
	public unsafe IntPtr GetProcAddress(IntPtr proc)
	{
		return _addr_GetProcAddress(proc);
	}

	[GetProcAddress("eglBindAPI")]
	public unsafe bool BindApi(int api)
	{
		return _addr_BindApi(api) != 0;
	}

	[GetProcAddress("eglChooseConfig")]
	public unsafe bool ChooseConfig(IntPtr display, int[] attribs, out IntPtr surfaceConfig, int numConfigs, out int choosenConfig)
	{
		fixed (int* ptr = attribs)
		{
			return _addr_ChooseConfig(display, ptr, out surfaceConfig, numConfigs, out choosenConfig) != 0;
		}
	}

	[GetProcAddress("eglCreateContext")]
	public unsafe IntPtr CreateContext(IntPtr display, IntPtr config, IntPtr share, int[] attrs)
	{
		fixed (int* ptr = attrs)
		{
			return _addr_CreateContext(display, config, share, ptr);
		}
	}

	[GetProcAddress("eglDestroyContext")]
	public unsafe bool DestroyContext(IntPtr display, IntPtr context)
	{
		return _addr_DestroyContext(display, context) != 0;
	}

	[GetProcAddress("eglCreatePbufferSurface")]
	public unsafe IntPtr CreatePBufferSurface(IntPtr display, IntPtr config, int[]? attrs)
	{
		fixed (int* ptr = attrs)
		{
			return _addr_CreatePBufferSurface(display, config, ptr);
		}
	}

	[GetProcAddress("eglMakeCurrent")]
	public unsafe bool MakeCurrent(IntPtr display, IntPtr draw, IntPtr read, IntPtr context)
	{
		return _addr_MakeCurrent(display, draw, read, context) != 0;
	}

	[GetProcAddress("eglGetCurrentContext")]
	public unsafe IntPtr GetCurrentContext()
	{
		return _addr_GetCurrentContext();
	}

	[GetProcAddress("eglGetCurrentDisplay")]
	public unsafe IntPtr GetCurrentDisplay()
	{
		return _addr_GetCurrentDisplay();
	}

	[GetProcAddress("eglGetCurrentSurface")]
	public unsafe IntPtr GetCurrentSurface(int readDraw)
	{
		return _addr_GetCurrentSurface(readDraw);
	}

	[GetProcAddress("eglDestroySurface")]
	public unsafe void DestroySurface(IntPtr display, IntPtr surface)
	{
		_addr_DestroySurface(display, surface);
	}

	[GetProcAddress("eglSwapBuffers")]
	public unsafe void SwapBuffers(IntPtr display, IntPtr surface)
	{
		_addr_SwapBuffers(display, surface);
	}

	[GetProcAddress("eglCreateWindowSurface")]
	public unsafe IntPtr CreateWindowSurface(IntPtr display, IntPtr config, IntPtr window, int[]? attrs)
	{
		fixed (int* ptr = attrs)
		{
			return _addr_CreateWindowSurface(display, config, window, ptr);
		}
	}

	[GetProcAddress("eglBindTexImage")]
	public unsafe int BindTexImage(IntPtr display, IntPtr surface, int buffer)
	{
		return _addr_BindTexImage(display, surface, buffer);
	}

	[GetProcAddress("eglGetConfigAttrib")]
	public unsafe bool GetConfigAttrib(IntPtr display, IntPtr config, int attr, out int rv)
	{
		return _addr_GetConfigAttrib(display, config, attr, out rv) != 0;
	}

	[GetProcAddress("eglWaitGL")]
	public unsafe bool WaitGL()
	{
		return _addr_WaitGL() != 0;
	}

	[GetProcAddress("eglWaitClient")]
	public unsafe bool WaitClient()
	{
		return _addr_WaitClient() != 0;
	}

	[GetProcAddress("eglWaitNative")]
	public unsafe bool WaitNative(int engine)
	{
		return _addr_WaitNative(engine) != 0;
	}

	[GetProcAddress("eglQueryString")]
	public unsafe IntPtr QueryStringNative(IntPtr display, int i)
	{
		return _addr_QueryStringNative(display, i);
	}

	public string? QueryString(IntPtr display, int i)
	{
		IntPtr intPtr = QueryStringNative(display, i);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		return Marshal.PtrToStringAnsi(intPtr);
	}

	[GetProcAddress("eglCreatePbufferFromClientBuffer")]
	public unsafe IntPtr CreatePbufferFromClientBuffer(IntPtr display, int buftype, IntPtr buffer, IntPtr config, int[]? attrib_list)
	{
		fixed (int* ptr = attrib_list)
		{
			return _addr_CreatePbufferFromClientBuffer(display, buftype, buffer, config, ptr);
		}
	}

	[GetProcAddress("eglQueryDisplayAttribEXT", true)]
	public unsafe bool QueryDisplayAttribExt(IntPtr display, int attr, out IntPtr res)
	{
		if (_addr_QueryDisplayAttribExt == (delegate* unmanaged[Stdcall]<IntPtr, int, out IntPtr, int>)null)
		{
			throw new EntryPointNotFoundException("QueryDisplayAttribExt");
		}
		return _addr_QueryDisplayAttribExt(display, attr, out res) != 0;
	}

	[GetProcAddress("eglQueryDeviceAttribEXT", true)]
	public unsafe bool QueryDeviceAttribExt(IntPtr display, int attr, out IntPtr res)
	{
		if (_addr_QueryDeviceAttribExt == (delegate* unmanaged[Stdcall]<IntPtr, int, out IntPtr, int>)null)
		{
			throw new EntryPointNotFoundException("QueryDeviceAttribExt");
		}
		return _addr_QueryDeviceAttribExt(display, attr, out res) != 0;
	}

	private unsafe void Initialize(Func<string, IntPtr> getProcAddress)
	{
		IntPtr zero = IntPtr.Zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglGetError");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetError");
		}
		_addr_GetError = (delegate* unmanaged[Stdcall]<int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglGetDisplay");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetDisplay");
		}
		_addr_GetDisplay = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglGetPlatformDisplayEXT");
		_addr_GetPlatformDisplayExt = (delegate* unmanaged[Stdcall]<int, IntPtr, int*, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglInitialize");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_Initialize");
		}
		_addr_Initialize = (delegate* unmanaged[Stdcall]<IntPtr, out int, out int, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglTerminate");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_Terminate");
		}
		_addr_Terminate = (delegate* unmanaged[Stdcall]<IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglGetProcAddress");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetProcAddress");
		}
		_addr_GetProcAddress = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglBindAPI");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_BindApi");
		}
		_addr_BindApi = (delegate* unmanaged[Stdcall]<int, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglChooseConfig");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_ChooseConfig");
		}
		_addr_ChooseConfig = (delegate* unmanaged[Stdcall]<IntPtr, int*, out IntPtr, int, out int, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglCreateContext");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreateContext");
		}
		_addr_CreateContext = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, int*, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglDestroyContext");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DestroyContext");
		}
		_addr_DestroyContext = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglCreatePbufferSurface");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreatePBufferSurface");
		}
		_addr_CreatePBufferSurface = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int*, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglMakeCurrent");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_MakeCurrent");
		}
		_addr_MakeCurrent = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, IntPtr, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglGetCurrentContext");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetCurrentContext");
		}
		_addr_GetCurrentContext = (delegate* unmanaged[Stdcall]<IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglGetCurrentDisplay");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetCurrentDisplay");
		}
		_addr_GetCurrentDisplay = (delegate* unmanaged[Stdcall]<IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglGetCurrentSurface");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetCurrentSurface");
		}
		_addr_GetCurrentSurface = (delegate* unmanaged[Stdcall]<int, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglDestroySurface");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DestroySurface");
		}
		_addr_DestroySurface = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglSwapBuffers");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_SwapBuffers");
		}
		_addr_SwapBuffers = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglCreateWindowSurface");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreateWindowSurface");
		}
		_addr_CreateWindowSurface = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, int*, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglBindTexImage");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_BindTexImage");
		}
		_addr_BindTexImage = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglGetConfigAttrib");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetConfigAttrib");
		}
		_addr_GetConfigAttrib = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int, out int, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglWaitGL");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_WaitGL");
		}
		_addr_WaitGL = (delegate* unmanaged[Stdcall]<int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglWaitClient");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_WaitClient");
		}
		_addr_WaitClient = (delegate* unmanaged[Stdcall]<int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglWaitNative");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_WaitNative");
		}
		_addr_WaitNative = (delegate* unmanaged[Stdcall]<int, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglQueryString");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_QueryStringNative");
		}
		_addr_QueryStringNative = (delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglCreatePbufferFromClientBuffer");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreatePbufferFromClientBuffer");
		}
		_addr_CreatePbufferFromClientBuffer = (delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr, IntPtr, int*, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglQueryDisplayAttribEXT");
		_addr_QueryDisplayAttribExt = (delegate* unmanaged[Stdcall]<IntPtr, int, out IntPtr, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("eglQueryDeviceAttribEXT");
		_addr_QueryDeviceAttribExt = (delegate* unmanaged[Stdcall]<IntPtr, int, out IntPtr, int>)(void*)zero;
	}
}
