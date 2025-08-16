using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("15fdf2e2-9847-41cd-95dd-510612a025ea")]
public enum RowOrColumnMajor
{
	RowMajor,
	ColumnMajor,
	Indeterminate
}
