using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("b17d6187-0907-464b-a168-0ef17a1572b1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IGridProvider
{
	int RowCount { get; }

	int ColumnCount { get; }

	IRawElementProviderSimple? GetItem(int row, int column);
}
