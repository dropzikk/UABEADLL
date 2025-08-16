using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Avalonia.Compatibility;

internal class NativeLibraryEx
{
	public static IntPtr Load(string dll, Assembly assembly)
	{
		return NativeLibrary.Load(dll, assembly, null);
	}

	public static IntPtr Load(string dll)
	{
		return NativeLibrary.Load(dll);
	}

	public static bool TryGetExport(IntPtr handle, string name, out IntPtr address)
	{
		return NativeLibrary.TryGetExport(handle, name, out address);
	}
}
