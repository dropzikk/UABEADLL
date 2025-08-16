using System.Runtime.InteropServices;

namespace Avalonia.X11;

[StructLayout(LayoutKind.Explicit)]
internal struct XIAnyHierarchyChangeInfo
{
	[FieldOffset(0)]
	public int type;

	[FieldOffset(4)]
	public XIAddMasterInfo add;

	[FieldOffset(4)]
	public XIRemoveMasterInfo remove;

	[FieldOffset(4)]
	public XIAttachSlaveInfo attach;

	[FieldOffset(4)]
	public XIDetachSlaveInfo detach;
}
