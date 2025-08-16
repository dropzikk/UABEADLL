using System;
using System.Runtime.InteropServices;

namespace Avalonia.Native;

internal class Win32Loader : IDynLoader
{
	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

	[DllImport("kernel32", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW", SetLastError = true)]
	private static extern IntPtr LoadLibrary(string lpszLib);

	IntPtr IDynLoader.LoadLibrary(string dll)
	{
		IntPtr intPtr = LoadLibrary(dll);
		if (intPtr != IntPtr.Zero)
		{
			return intPtr;
		}
		throw new Exception("Error loading " + dll + " error " + Marshal.GetLastWin32Error());
	}

	IntPtr IDynLoader.GetProcAddress(IntPtr dll, string proc, bool optional)
	{
		IntPtr procAddress = GetProcAddress(dll, proc);
		if (procAddress == IntPtr.Zero && !optional)
		{
			throw new Exception("Error " + Marshal.GetLastWin32Error());
		}
		return procAddress;
	}
}
