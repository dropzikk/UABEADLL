using System;

namespace Avalonia.X11;

internal struct XIDeviceInfo
{
	public int Deviceid;

	public IntPtr Name;

	public XiDeviceType Use;

	public int Attachment;

	public bool Enabled;

	public int NumClasses;

	public unsafe XIAnyClassInfo** Classes;
}
