using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("670c3006-bf4c-428b-8534-e1848f645122")]
public enum NavigateDirection
{
	Parent,
	NextSibling,
	PreviousSibling,
	FirstChild,
	LastChild
}
