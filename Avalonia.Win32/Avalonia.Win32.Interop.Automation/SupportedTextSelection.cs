using System;
using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[Flags]
[ComVisible(true)]
[Guid("3d9e3d8f-bfb0-484f-84ab-93ff4280cbc4")]
public enum SupportedTextSelection
{
	None = 0,
	Single = 1,
	Multiple = 2
}
