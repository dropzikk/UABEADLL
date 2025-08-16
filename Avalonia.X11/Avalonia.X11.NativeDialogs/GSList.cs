using System;

namespace Avalonia.X11.NativeDialogs;

internal struct GSList
{
	public readonly IntPtr Data;

	public unsafe readonly GSList* Next;
}
