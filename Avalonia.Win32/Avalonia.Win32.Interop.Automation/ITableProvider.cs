using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("9c860395-97b3-490a-b52a-858cc22af166")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITableProvider : IGridProvider
{
	RowOrColumnMajor RowOrColumnMajor { get; }

	IRawElementProviderSimple[] GetRowHeaders();

	IRawElementProviderSimple[] GetColumnHeaders();
}
