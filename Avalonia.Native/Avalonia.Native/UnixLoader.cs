using System;
using System.Runtime.InteropServices;

namespace Avalonia.Native;

internal class UnixLoader : IDynLoader
{
	private static class LinuxImports
	{
		[DllImport("libdl.so.2")]
		private static extern IntPtr dlopen(string path, int flags);

		[DllImport("libdl.so.2")]
		private static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("libdl.so.2")]
		private static extern IntPtr dlerror();

		public static void Init()
		{
			DlOpen = dlopen;
			DlSym = dlsym;
			DlError = dlerror;
		}
	}

	private static class OsXImports
	{
		[DllImport("/usr/lib/libSystem.dylib")]
		private static extern IntPtr dlopen(string path, int flags);

		[DllImport("/usr/lib/libSystem.dylib")]
		private static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("/usr/lib/libSystem.dylib")]
		private static extern IntPtr dlerror();

		public static void Init()
		{
			DlOpen = dlopen;
			DlSym = dlsym;
			DlError = dlerror;
		}
	}

	private static Func<string, int, IntPtr> DlOpen;

	private static Func<IntPtr, string, IntPtr> DlSym;

	private static Func<IntPtr> DlError;

	[DllImport("libc")]
	private static extern int uname(IntPtr buf);

	static UnixLoader()
	{
		IntPtr intPtr = Marshal.AllocHGlobal(4096);
		uname(intPtr);
		string text = Marshal.PtrToStringAnsi(intPtr);
		Marshal.FreeHGlobal(intPtr);
		if (text == "Darwin")
		{
			OsXImports.Init();
		}
		else
		{
			LinuxImports.Init();
		}
	}

	private static string DlErrorString()
	{
		return Marshal.PtrToStringAnsi(DlError());
	}

	public IntPtr LoadLibrary(string dll)
	{
		IntPtr intPtr = DlOpen(dll, 1);
		if (intPtr == IntPtr.Zero)
		{
			throw new Exception(DlErrorString());
		}
		return intPtr;
	}

	public IntPtr GetProcAddress(IntPtr dll, string proc, bool optional)
	{
		IntPtr intPtr = DlSym(dll, proc);
		if (intPtr == IntPtr.Zero && !optional)
		{
			throw new Exception(DlErrorString());
		}
		return intPtr;
	}
}
