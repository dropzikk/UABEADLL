using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Avalonia.OpenGL;
using Avalonia.Threading;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32.OpenGl;

internal class WglDisplay
{
	private delegate bool WglChoosePixelFormatARBDelegate(IntPtr hdc, int[]? piAttribIList, float[]? pfAttribFList, int nMaxFormats, int[] piFormats, out int nNumFormats);

	private delegate IntPtr WglCreateContextAttribsARBDelegate(IntPtr hDC, IntPtr hShareContext, int[]? attribList);

	private delegate void GlDebugMessageCallbackDelegate(IntPtr callback, IntPtr userParam);

	private delegate void DebugCallbackDelegate(int source, int type, int id, int severity, int len, IntPtr message, IntPtr userParam);

	private static bool? _initialized;

	private static readonly DebugCallbackDelegate _debugCallback = DebugCallback;

	private static IntPtr _bootstrapContext;

	private static IntPtr _bootstrapWindow;

	private static IntPtr _bootstrapDc;

	private static PixelFormatDescriptor _defaultPfd;

	private static int _defaultPixelFormat;

	public static IntPtr OpenGl32Handle = UnmanagedMethods.LoadLibrary("opengl32");

	private static WglChoosePixelFormatARBDelegate? s_wglChoosePixelFormatArb;

	private static WglCreateContextAttribsARBDelegate? s_wglCreateContextAttribsArb;

	private static GlDebugMessageCallbackDelegate? s_glDebugMessageCallback;

	[MemberNotNullWhen(true, "s_wglChoosePixelFormatArb")]
	[MemberNotNullWhen(true, "s_wglCreateContextAttribsArb")]
	private static bool Initialize()
	{
		bool valueOrDefault = _initialized == true;
		if (!_initialized.HasValue)
		{
			valueOrDefault = InitializeCore();
			_initialized = valueOrDefault;
			return valueOrDefault;
		}
		return valueOrDefault;
	}

	[MemberNotNullWhen(true, "s_wglChoosePixelFormatArb")]
	[MemberNotNullWhen(true, "s_wglCreateContextAttribsArb")]
	private static bool InitializeCore()
	{
		Dispatcher.UIThread.VerifyAccess();
		_bootstrapWindow = WglGdiResourceManager.CreateOffscreenWindow();
		_bootstrapDc = WglGdiResourceManager.GetDC(_bootstrapWindow);
		PixelFormatDescriptor defaultPfd = default(PixelFormatDescriptor);
		defaultPfd.Size = (ushort)Marshal.SizeOf<PixelFormatDescriptor>();
		defaultPfd.Version = 1;
		defaultPfd.Flags = PixelFormatDescriptorFlags.PFD_DOUBLEBUFFER | PixelFormatDescriptorFlags.PFD_DRAW_TO_WINDOW | PixelFormatDescriptorFlags.PFD_SUPPORT_OPENGL;
		defaultPfd.DepthBits = 24;
		defaultPfd.StencilBits = 8;
		defaultPfd.ColorBits = 32;
		_defaultPfd = defaultPfd;
		_defaultPixelFormat = UnmanagedMethods.ChoosePixelFormat(_bootstrapDc, ref _defaultPfd);
		UnmanagedMethods.SetPixelFormat(_bootstrapDc, _defaultPixelFormat, ref _defaultPfd);
		_bootstrapContext = UnmanagedMethods.wglCreateContext(_bootstrapDc);
		if (_bootstrapContext == IntPtr.Zero)
		{
			return false;
		}
		UnmanagedMethods.wglMakeCurrent(_bootstrapDc, _bootstrapContext);
		s_wglCreateContextAttribsArb = Marshal.GetDelegateForFunctionPointer<WglCreateContextAttribsARBDelegate>(UnmanagedMethods.wglGetProcAddress("wglCreateContextAttribsARB"));
		s_wglChoosePixelFormatArb = Marshal.GetDelegateForFunctionPointer<WglChoosePixelFormatARBDelegate>(UnmanagedMethods.wglGetProcAddress("wglChoosePixelFormatARB"));
		IntPtr intPtr = UnmanagedMethods.wglGetProcAddress("glDebugMessageCallback");
		s_glDebugMessageCallback = ((intPtr != (IntPtr)0) ? Marshal.GetDelegateForFunctionPointer<GlDebugMessageCallbackDelegate>(intPtr) : null);
		int[] array = new int[1];
		s_wglChoosePixelFormatArb(_bootstrapDc, new int[19]
		{
			8193, 1, 8195, 8231, 8208, 1, 8209, 1, 8211, 8235,
			8212, 32, 8219, 8, 8226, 0, 8227, 0, 0
		}, null, 1, array, out var nNumFormats);
		if (nNumFormats != 0)
		{
			UnmanagedMethods.DescribePixelFormat(_bootstrapDc, array[0], Marshal.SizeOf<PixelFormatDescriptor>(), ref _defaultPfd);
			_defaultPixelFormat = array[0];
		}
		UnmanagedMethods.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		return true;
	}

	private static void DebugCallback(int source, int type, int id, int severity, int len, IntPtr message, IntPtr userparam)
	{
		string value = Marshal.PtrToStringAnsi(message, len);
		Console.Error.WriteLine(value);
	}

	public static WglContext? CreateContext(GlVersion[] versions, IGlContext? share)
	{
		if (!Initialize())
		{
			return null;
		}
		WglContext wglContext = share as WglContext;
		using (new WglRestoreContext(_bootstrapDc, _bootstrapContext, null))
		{
			IntPtr hWnd = WglGdiResourceManager.CreateOffscreenWindow();
			IntPtr dC = WglGdiResourceManager.GetDC(hWnd);
			UnmanagedMethods.SetPixelFormat(dC, _defaultPixelFormat, ref _defaultPfd);
			for (int i = 0; i < versions.Length; i++)
			{
				GlVersion version = versions[i];
				if (version.Type != 0)
				{
					continue;
				}
				IntPtr intPtr;
				using (wglContext?.Lock())
				{
					WglCreateContextAttribsARBDelegate? wglCreateContextAttribsARBDelegate = s_wglCreateContextAttribsArb;
					IntPtr hShareContext = wglContext?.Handle ?? IntPtr.Zero;
					int[] obj = new int[8] { 8337, 0, 8338, 0, 37158, 1, 0, 0 };
					obj[1] = version.Major;
					obj[3] = version.Minor;
					intPtr = wglCreateContextAttribsARBDelegate(dC, hShareContext, obj);
				}
				if (s_glDebugMessageCallback != null)
				{
					using (new WglRestoreContext(dC, intPtr, null))
					{
						s_glDebugMessageCallback(Marshal.GetFunctionPointerForDelegate(_debugCallback), IntPtr.Zero);
					}
				}
				if (intPtr != IntPtr.Zero)
				{
					return new WglContext(wglContext, version, intPtr, hWnd, dC, _defaultPixelFormat, _defaultPfd);
				}
			}
			WglGdiResourceManager.ReleaseDC(hWnd, dC);
			WglGdiResourceManager.DestroyWindow(hWnd);
			return null;
		}
	}
}
