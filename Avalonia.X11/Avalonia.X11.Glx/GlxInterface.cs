using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.SourceGenerator;

namespace Avalonia.X11.Glx;

internal class GlxInterface
{
	private const string libGL = "libGL.so.1";

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, IntPtr, int> _addr_MakeContextCurrent;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr> _addr_GetCurrentContext;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr> _addr_GetCurrentDisplay;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr> _addr_GetCurrentDrawable;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr> _addr_GetCurrentReadDrawable;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int*, IntPtr> _addr_CreatePbuffer;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr> _addr_DestroyPbuffer;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, int, int*, XVisualInfo*> _addr_ChooseVisual;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, XVisualInfo*, IntPtr, int, IntPtr> _addr_CreateContext;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, int, int*, IntPtr> _addr_CreateContextAttribsARB;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, void> _addr_DestroyContext;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, int, int*, out int, IntPtr*> _addr_ChooseFBConfig;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, XVisualInfo*> _addr_GetVisualFromFBConfig;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int, out int, int> _addr_GetFBConfigAttrib;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, void> _addr_SwapBuffers;

	private unsafe delegate* unmanaged[Stdcall]<void> _addr_WaitX;

	private unsafe delegate* unmanaged[Stdcall]<void> _addr_WaitGL;

	private unsafe delegate* unmanaged[Stdcall]<int> _addr_GlGetError;

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr> _addr_QueryExtensionsString;

	[GetProcAddress("glXMakeContextCurrent")]
	public unsafe bool MakeContextCurrent(IntPtr display, IntPtr draw, IntPtr read, IntPtr context)
	{
		return _addr_MakeContextCurrent(display, draw, read, context) != 0;
	}

	[GetProcAddress("glXGetCurrentContext")]
	public unsafe IntPtr GetCurrentContext()
	{
		return _addr_GetCurrentContext();
	}

	[GetProcAddress("glXGetCurrentDisplay")]
	public unsafe IntPtr GetCurrentDisplay()
	{
		return _addr_GetCurrentDisplay();
	}

	[GetProcAddress("glXGetCurrentDrawable")]
	public unsafe IntPtr GetCurrentDrawable()
	{
		return _addr_GetCurrentDrawable();
	}

	[GetProcAddress("glXGetCurrentReadDrawable")]
	public unsafe IntPtr GetCurrentReadDrawable()
	{
		return _addr_GetCurrentReadDrawable();
	}

	[GetProcAddress("glXCreatePbuffer")]
	public unsafe IntPtr CreatePbuffer(IntPtr dpy, IntPtr fbc, int[] attrib_list)
	{
		fixed (int* ptr = attrib_list)
		{
			return _addr_CreatePbuffer(dpy, fbc, ptr);
		}
	}

	[GetProcAddress("glXDestroyPbuffer")]
	public unsafe IntPtr DestroyPbuffer(IntPtr dpy, IntPtr fb)
	{
		return _addr_DestroyPbuffer(dpy, fb);
	}

	[GetProcAddress("glXChooseVisual")]
	public unsafe XVisualInfo* ChooseVisual(IntPtr dpy, int screen, int[] attribList)
	{
		fixed (int* ptr = attribList)
		{
			return _addr_ChooseVisual(dpy, screen, ptr);
		}
	}

	[GetProcAddress("glXCreateContext")]
	public unsafe IntPtr CreateContext(IntPtr dpy, XVisualInfo* vis, IntPtr shareList, bool direct)
	{
		return _addr_CreateContext(dpy, vis, shareList, direct ? 1 : 0);
	}

	[GetProcAddress("glXCreateContextAttribsARB")]
	public unsafe IntPtr CreateContextAttribsARB(IntPtr dpy, IntPtr fbconfig, IntPtr shareList, bool direct, int[] attribs)
	{
		fixed (int* ptr = attribs)
		{
			return _addr_CreateContextAttribsARB(dpy, fbconfig, shareList, direct ? 1 : 0, ptr);
		}
	}

	[DllImport("libGL.so.1", EntryPoint = "glXGetProcAddress")]
	public static extern IntPtr GlxGetProcAddress(string buffer);

	[GetProcAddress("glXDestroyContext")]
	public unsafe void DestroyContext(IntPtr dpy, IntPtr ctx)
	{
		_addr_DestroyContext(dpy, ctx);
	}

	[GetProcAddress("glXChooseFBConfig")]
	public unsafe IntPtr* ChooseFBConfig(IntPtr dpy, int screen, int[] attrib_list, out int nelements)
	{
		fixed (int* ptr = attrib_list)
		{
			return _addr_ChooseFBConfig(dpy, screen, ptr, out nelements);
		}
	}

	public unsafe IntPtr* ChooseFbConfig(IntPtr dpy, int screen, IEnumerable<int> attribs, out int nelements)
	{
		int[] attrib_list = attribs.Concat(new int[1]).ToArray();
		return ChooseFBConfig(dpy, screen, attrib_list, out nelements);
	}

	[GetProcAddress("glXGetVisualFromFBConfig")]
	public unsafe XVisualInfo* GetVisualFromFBConfig(IntPtr dpy, IntPtr config)
	{
		return _addr_GetVisualFromFBConfig(dpy, config);
	}

	[GetProcAddress("glXGetFBConfigAttrib")]
	public unsafe int GetFBConfigAttrib(IntPtr dpy, IntPtr config, int attribute, out int value)
	{
		return _addr_GetFBConfigAttrib(dpy, config, attribute, out value);
	}

	[GetProcAddress("glXSwapBuffers")]
	public unsafe void SwapBuffers(IntPtr dpy, IntPtr drawable)
	{
		_addr_SwapBuffers(dpy, drawable);
	}

	[GetProcAddress("glXWaitX")]
	public unsafe void WaitX()
	{
		_addr_WaitX();
	}

	[GetProcAddress("glXWaitGL")]
	public unsafe void WaitGL()
	{
		_addr_WaitGL();
	}

	[GetProcAddress("glGetError")]
	public unsafe int GlGetError()
	{
		return _addr_GlGetError();
	}

	[GetProcAddress("glXQueryExtensionsString")]
	public unsafe IntPtr QueryExtensionsString(IntPtr display, int screen)
	{
		return _addr_QueryExtensionsString(display, screen);
	}

	public GlxInterface()
	{
		Initialize(SafeGetProcAddress);
	}

	public static IntPtr SafeGetProcAddress(string proc)
	{
		if (proc.StartsWith("egl", StringComparison.InvariantCulture))
		{
			return IntPtr.Zero;
		}
		return GlxGetProcAddress(proc);
	}

	public string[] GetExtensions(IntPtr display)
	{
		return (from x in Marshal.PtrToStringAnsi(QueryExtensionsString(display, 0)).Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
			select x.Trim()).ToArray();
	}

	private unsafe void Initialize(Func<string, IntPtr> getProcAddress)
	{
		IntPtr zero = IntPtr.Zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXMakeContextCurrent");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_MakeContextCurrent");
		}
		_addr_MakeContextCurrent = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, IntPtr, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXGetCurrentContext");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetCurrentContext");
		}
		_addr_GetCurrentContext = (delegate* unmanaged[Stdcall]<IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXGetCurrentDisplay");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetCurrentDisplay");
		}
		_addr_GetCurrentDisplay = (delegate* unmanaged[Stdcall]<IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXGetCurrentDrawable");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetCurrentDrawable");
		}
		_addr_GetCurrentDrawable = (delegate* unmanaged[Stdcall]<IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXGetCurrentReadDrawable");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetCurrentReadDrawable");
		}
		_addr_GetCurrentReadDrawable = (delegate* unmanaged[Stdcall]<IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXCreatePbuffer");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreatePbuffer");
		}
		_addr_CreatePbuffer = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int*, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXDestroyPbuffer");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DestroyPbuffer");
		}
		_addr_DestroyPbuffer = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXChooseVisual");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_ChooseVisual");
		}
		_addr_ChooseVisual = (delegate* unmanaged[Stdcall]<IntPtr, int, int*, XVisualInfo*>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXCreateContext");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreateContext");
		}
		_addr_CreateContext = (delegate* unmanaged[Stdcall]<IntPtr, XVisualInfo*, IntPtr, int, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXCreateContextAttribsARB");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreateContextAttribsARB");
		}
		_addr_CreateContextAttribsARB = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, int, int*, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXDestroyContext");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DestroyContext");
		}
		_addr_DestroyContext = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXChooseFBConfig");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_ChooseFBConfig");
		}
		_addr_ChooseFBConfig = (delegate* unmanaged[Stdcall]<IntPtr, int, int*, out int, IntPtr*>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXGetVisualFromFBConfig");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetVisualFromFBConfig");
		}
		_addr_GetVisualFromFBConfig = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, XVisualInfo*>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXGetFBConfigAttrib");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetFBConfigAttrib");
		}
		_addr_GetFBConfigAttrib = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int, out int, int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXSwapBuffers");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_SwapBuffers");
		}
		_addr_SwapBuffers = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXWaitX");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_WaitX");
		}
		_addr_WaitX = (delegate* unmanaged[Stdcall]<void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXWaitGL");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_WaitGL");
		}
		_addr_WaitGL = (delegate* unmanaged[Stdcall]<void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetError");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GlGetError");
		}
		_addr_GlGetError = (delegate* unmanaged[Stdcall]<int>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glXQueryExtensionsString");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_QueryExtensionsString");
		}
		_addr_QueryExtensionsString = (delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr>)(void*)zero;
	}
}
