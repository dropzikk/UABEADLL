using System;

namespace Avalonia.Native;

internal interface IDynLoader
{
	IntPtr LoadLibrary(string dll);

	IntPtr GetProcAddress(IntPtr dll, string proc, bool optional);
}
