using System;
using System.Runtime.InteropServices.ComTypes;

namespace Avalonia.Win32.Interop;

internal struct FORMATETC
{
	public ushort cfFormat;

	public IntPtr ptd;

	public DVASPECT dwAspect;

	public int lindex;

	public TYMED tymed;
}
