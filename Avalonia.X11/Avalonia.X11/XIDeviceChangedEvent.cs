using System;

namespace Avalonia.X11;

internal struct XIDeviceChangedEvent
{
	public int Type;

	public UIntPtr Serial;

	public bool SendEvent;

	public IntPtr Display;

	public int Extension;

	public int Evtype;

	public IntPtr Time;

	public int Deviceid;

	public int Sourceid;

	public int Reason;

	public int NumClasses;

	public unsafe XIAnyClassInfo** Classes;
}
