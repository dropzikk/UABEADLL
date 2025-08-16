using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("65101cc7-7904-408e-87a7-8c6dbd83a18b")]
public enum WindowInteractionState
{
	Running,
	Closing,
	ReadyForUserInteraction,
	BlockedByModalWindow,
	NotResponding
}
