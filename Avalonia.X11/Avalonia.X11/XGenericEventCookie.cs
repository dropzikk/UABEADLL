using System;
using System.Runtime.CompilerServices;

namespace Avalonia.X11;

internal struct XGenericEventCookie
{
	internal int type;

	internal IntPtr serial;

	internal int send_event;

	internal IntPtr display;

	internal int extension;

	internal int evtype;

	internal uint cookie;

	internal unsafe void* data;

	public unsafe T GetEvent<T>() where T : unmanaged
	{
		if (data == null)
		{
			throw new InvalidOperationException();
		}
		return Unsafe.ReadUnaligned<T>(data);
	}
}
