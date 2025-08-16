using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("e4cfef41-071d-472c-a65c-c14f59ea81eb")]
internal enum StructureChangeType
{
	ChildAdded,
	ChildRemoved,
	ChildrenInvalidated,
	ChildrenBulkAdded,
	ChildrenBulkRemoved,
	ChildrenReordered
}
