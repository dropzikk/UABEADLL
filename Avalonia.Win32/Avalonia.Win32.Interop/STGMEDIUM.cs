using System;
using System.Runtime.InteropServices.ComTypes;

namespace Avalonia.Win32.Interop;

internal struct STGMEDIUM
{
	public TYMED tymed;

	public IntPtr unionmember;

	public IntPtr pUnkForRelease;
}
