using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("d8e55844-7043-4edc-979d-593cc6b4775e")]
internal enum AsyncContentLoadedState
{
	Beginning,
	Progress,
	Completed
}
